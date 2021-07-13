// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.UnitTesting;
using System.Linq;
using System.Reflection;
using System.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integration
{
    [TestClass]
    public class DiscoveryTests
    {
        public abstract class AbstractClassWithExports
        {
            [Export("StaticExport")]
            public static string StaticExport { get { return "ExportedValue"; } }

            [Export("InstanceExport")]
            public string InstanceExport { get { return "InstanceExportedValue"; } }
        }

        [TestMethod]
        public void Export_StaticOnAbstractClass_ShouldExist()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(AbstractClassWithExports));

            Assert.IsTrue(container.IsPresent("StaticExport"));
            Assert.IsFalse(container.IsPresent("InstanceExport"));
        }

        public class ClassWithStaticImport
        {
            [Import("StaticImport")]
            public static string MyImport
            {
                get; set;
            }
        }

        [TestMethod]
        public void Import_StaticImport_ShouldNotBeSet()
        {
            var container = ContainerFactory.Create();
            container.AddAndComposeExportedObject("StaticImport", "String that shouldn't be imported");

            var importer = new ClassWithStaticImport();

            container.SatisfyImports(importer);

            Assert.IsNull(ClassWithStaticImport.MyImport, "Static import should not have been set!");
        }

#if !SILVERLIGHT
// private imports don't work on SILVERLIGHT
        [Export]
        public class BaseWithNonPublicImportAndExport
        {
            [Import("BasePrivateImport")]
            private string _basePrivateImport = null;

            public string BasePrivateImport { get { return this._basePrivateImport; } }
        }

        [Export]
        public class DerivedBaseWithNonPublicImportAndExport : BaseWithNonPublicImportAndExport
        {

        }

        [TestMethod]
        public void Import_PrivateOnClass_ShouldSetImport()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(BaseWithNonPublicImportAndExport));
            container.AddAndComposeExportedObject("BasePrivateImport", "Imported String");

            var importer = container.GetExportedObject<BaseWithNonPublicImportAndExport>();
            Assert.AreEqual("Imported String", importer.BasePrivateImport);
        }


        [TestMethod]
        public void Import_PrivateOnBase_ShouldSetImport()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(DerivedBaseWithNonPublicImportAndExport));
            container.AddAndComposeExportedObject("BasePrivateImport", "Imported String");

            var importer = container.GetExportedObject<DerivedBaseWithNonPublicImportAndExport>();
            Assert.AreEqual("Imported String", importer.BasePrivateImport);
        }
#endif // !SILVERLIGHT

        [PartExportsInherited]
        public abstract class BaseTypeWithStaticExport
        {
            [Export("StaticExport")]
            public static int StaticExport = 25;
        }

        public class DerivedBaseTypeWithStaticExport : BaseTypeWithStaticExport
        {
        }

        [TestMethod]
        public void Export_StaticExportOnBaseType_ShouldBeFoundOnlyOnce()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(BaseTypeWithStaticExport),
                typeof(DerivedBaseTypeWithStaticExport));

            var exports = container.GetExports<int>("StaticExport");

            Assert.AreEqual(1, exports.Count());
        }

        public interface InterfaceWithImport
        {
            [Import("InterfaceImport")]
            int MyImport { get; set; }
        }

        public interface InterfaceWithExport
        {
            [Export("InterfaceExport")]
            int MyExport { get; set; }
        }

        [TestMethod]
        public void AttributesOnInterface_ShouldNotBeConsiderAPart()
        {
            var catalog = CatalogFactory.CreateAttributed(
                typeof(InterfaceWithImport),
                typeof(InterfaceWithExport));

            Assert.AreEqual(0, catalog.Parts.Count());
        }

        [Export]
        public class ClassWithInterfaceInheritedImport : InterfaceWithImport
        {
            public int MyImport { get; set; }
        }

        [TestMethod]
        public void Import_InheritImportFromInterface_ShouldExposeImport()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(ClassWithInterfaceInheritedImport));

            container.AddAndComposeExportedObject("InterfaceImport", 42);

            var importer = container.GetExportedObject<ClassWithInterfaceInheritedImport>();

            Assert.AreEqual(42, importer.MyImport, "This import should get discovered from the interface");
        }

        public class ClassWithInterfaceInheritedExport : InterfaceWithExport
        {
            public ClassWithInterfaceInheritedExport()
            {
                MyExport = 42;
            }

            public int MyExport { get; set; }
        }

        [TestMethod]
        public void Import_InheritExportFromInterface_ShouldNotExposeExport()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(ClassWithInterfaceInheritedExport));

            Assert.IsFalse(container.IsPresent("InterfaceExport"), "Export defined on interface should not be discovered!");
        }

        public interface IFoo { }

        [Export]
        [PartExportsInherited]
        public abstract class BaseWithVirtualExport
        {
            [Export]
            public virtual IFoo MyProp { get; set; }
        }

        [Export(typeof(BaseWithVirtualExport))]
        public class DerivedWithOverrideExport : BaseWithVirtualExport
        {
            [Export]
            public override IFoo MyProp { get; set; }
        }

        [TestMethod]
        public void Export_BaseAndDerivedShouldAmountInTwoExports()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(BaseWithVirtualExport),
                typeof(DerivedWithOverrideExport));

            var exports1 = container.GetExportedObjects<BaseWithVirtualExport>();
            Assert.AreEqual(2, exports1.Count);

            var exports2 = container.GetExportedObjects<IFoo>();
            Assert.AreEqual(2, exports2.Count);
        }

        public interface IDocument { }

        [Export(typeof(IDocument))]
        [ExportMetadata("Name", "TextDocument")]
        public class TextDocument : IDocument
        {
        }

        [Export(typeof(IDocument))]
        [ExportMetadata("Name", "XmlDocument")]
        public class XmlDocument : TextDocument
        {
        }

        [TestMethod]
        public void Export_ExportingSameContractInDerived_ShouldResultInHidingBaseExport()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(IDocument),
                typeof(XmlDocument));

            var export = container.GetExport<IDocument>();

            Assert.AreEqual("XmlDocument", export.Metadata["Name"]);
        }

        [TestMethod]
        public void Export_ExportingBaseAndDerivedSameContract_ShouldResultInOnlyTwoExports()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(IDocument),
                typeof(TextDocument),
                typeof(XmlDocument));

            var exports = container.GetExports<IDocument>();

            Assert.AreEqual(2, exports.Count);
            Assert.AreEqual("TextDocument", exports[0].Metadata["Name"]);
            Assert.IsInstanceOfType(exports[0].GetExportedObject(), typeof(TextDocument));
            
            Assert.AreEqual("XmlDocument", exports[1].Metadata["Name"]);
            Assert.IsInstanceOfType(exports[1].GetExportedObject(), typeof(XmlDocument));
        }

        public interface IObjectSerializer { }

        [Export(typeof(IDocument))]
        [Export(typeof(IObjectSerializer))]
        [ExportMetadata("Name", "XamlDocument")]
        public class XamlDocument : XmlDocument, IObjectSerializer
        {
        }

        [TestMethod]
        public void Export_ExportingSameContractInDerivedAndNewContract_ShouldResultInHidingBaseAndExportingNewContract()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(XamlDocument));

            var export = container.GetExport<IDocument>();

            Assert.AreEqual("XamlDocument", export.Metadata["Name"]);

            var export2 = container.GetExport<IObjectSerializer>();

            Assert.AreEqual("XamlDocument", export2.Metadata["Name"]);
        }


        [Export(typeof(IDocument))]
        [ExportMetadata("Name", "WPFDocument")]
        public class WPFDocument : XamlDocument
        {
        }

        [TestMethod]
        public void Export_ExportingSameContractInDerivedAndAnotherContractInBase_ShouldResultInHidingOneBaseAndInheritingNewContract()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(WPFDocument));

            var export = container.GetExport<IDocument>();

            Assert.AreEqual("WPFDocument", export.Metadata["Name"]);

            var export2 = container.GetExportedObjectOrDefault<IObjectSerializer>();

            Assert.IsNull(export2, "IObjectSerializer export should not have been inherited");
        }
       
        [Export]
        [PartExportsInherited]
        public abstract class Plugin
        {
#if !SILVERLIGHT
            [Export("PluginFrameworkVersion")]
            private int _pluginFrameworkVersion = 1;

            [Export("PluginBaseLocation")]
            private string GetPluginBaseLocation
            {
                get
                {
                    return "BaseLocation";
                }
            }
#endif

            [Export("PluginLocation")]
            public virtual string GetLocation()
            {
                return "NoWhere";
            }

            [Export("PluginVersion")]
            public virtual int Version
            {
                get
                {
                    return 0;
                }
            }
        }

        private void VerifyValidPlugin(CompositionContainer container, int version, string location)
        {
            var plugins = container.GetExports<Plugin>();
            Assert.AreEqual(1, plugins.Count);

#if !SILVERLIGHT
            var fxVer = container.GetExportedObject<int>("PluginFrameworkVersion");
            Assert.AreEqual(1, fxVer);

            var fxLoc = container.GetExportedObject<string>("PluginBaseLocation");
            Assert.AreEqual("BaseLocation", fxLoc);
#endif

            var plVer = container.GetExportedObject<int>("PluginVersion");
            Assert.AreEqual(version, plVer);

            var plLoc = container.GetExportedObject<Func<string>>("PluginLocation");
            Assert.AreEqual(location, plLoc());
        }

        public class Plugin1 : Plugin
        {
        }

        [TestMethod]
        public void Export_Plugin1()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(Plugin1));

            VerifyValidPlugin(container, 0, "NoWhere");
        }

        public class Plugin2 : Plugin
        {
            public override string GetLocation()
            {
                return "SomeWhere";
            }
            public override int Version
            {
                get
                {
                    return 1;
                }
            }
        }

        [TestMethod]
        public void Export_Plugin2()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(Plugin2));

            VerifyValidPlugin(container, 1, "SomeWhere");
        }

        public class Plugin3 : Plugin
        {
            [Export("PluginLocation")]
            public override string GetLocation()
            {
                return "SomeWhere";
            }

            [Export("PluginVersion")]
            public override int Version
            {
                get
                {
                    return 1;
                }
            }
        }

        [TestMethod]
        public void Export_Plugin3()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(Plugin3));

            // Should get duplicate "PluginLocation"
            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
                container.GetExportedObject<int>("PluginLocation"));

            // Should get duplicate "PluginVersion"
            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
                container.GetExportedObject<int>("PluginVersion"));
        }

        public class Plugin4 : Plugin
        {
            [Export("MyPluginLocation")]
            public override string GetLocation()
            {
                return "SomeWhere4";
            }

            [Export("MyPluginVersion")]
            public override int Version
            {
                get
                {
                    return 4;
                }
            }
        }

        [TestMethod]
        public void Export_Plugin4()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(Plugin4));

            VerifyValidPlugin(container, 4, "SomeWhere4");

            var plVer = container.GetExportedObject<int>("MyPluginVersion");
            Assert.AreEqual(4, plVer);

            var plLoc = container.GetExportedObject<Func<string>>("MyPluginLocation");
            Assert.AreEqual("SomeWhere4", plLoc());
        }

        public class Plugin5 : Plugin
        {
            [Export("PluginLocation")]
            public new string GetLocation()
            {
                return "SomeWhere5";
            }

            [Export("PluginVersion")]
            [ExportMetadata("Version", 5)]
            public new int Version
            {
                get
                {
                    return 5;
                }
            }
        }

        [TestMethod]
        public void Export_Plugin5()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(Plugin5));

            var plVers = container.GetExports<int>("PluginVersion");

            Assert.AreEqual(2, plVers.Count);
            Assert.AreEqual(5, plVers[0].GetExportedObject());
            Assert.AreEqual(5, plVers[0].Metadata["Version"]);
            Assert.AreEqual(0, plVers[1].GetExportedObject());

            var plLocs = container.GetExportedObjects<Func<string>>("PluginLocation");
            Assert.AreEqual("SomeWhere5", plLocs[0]());
            Assert.AreEqual("NoWhere", plLocs[1]());
        }

        
        public interface IPlugin
        {
            [Export("PluginId")]
            int Id { get; }
        }

        
        public class MyPlugin : IPlugin
        {
            [Export("PluginId")]
            public int Id { get { return 0; } }
        }

        [TestMethod]
        public void Export_MyPlugin()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(MyPlugin));

            var export = container.GetExportedObject<int>("PluginId");
        }

        [Export]
        [PartExportsInherited]
        public interface IApplicationPlugin
        {
            [Export("ApplicationPluginNames")]
            string Name { get; }

            [Import("Application")]
            object Application { get; set; }
        }

        [Export]
        [PartExportsInherited]
        public interface IToolbarPlugin : IApplicationPlugin
        {
            [Import("ToolBar")]
            object ToolBar { get; set; }
        }

        public class MyToolbarPlugin : IToolbarPlugin
        {
            public string Name { get { return "MyToolbarPlugin"; } }

            public object Application { get; set; }

            public object ToolBar { get; set; }
        }

        [TestMethod]
        public void TestInterfaces()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(MyToolbarPlugin));

            var app = new object();
            container.AddAndComposeExportedObject<object>("Application", app);

            var toolbar = new object();
            container.AddAndComposeExportedObject<object>("ToolBar", toolbar);

            var export = container.GetExportedObject<IToolbarPlugin>();

            Assert.AreEqual(app, export.Application);
            Assert.AreEqual(toolbar, export.ToolBar);
            Assert.AreEqual("MyToolbarPlugin", export.Name);

            var pluginNames = container.GetExportedObjects<string>("ApplicationPluginNames");
            Assert.AreEqual(1, pluginNames.Count);
        }

        public class ImportOnVirtualProperty
        {
            public int ImportSetCount = 0;
            private int _value;

            [Import("VirtualImport")]
            public virtual int VirtualImport 
            {
                get
                {
                    return this._value;
                }
                set
                {
                    this._value = value;
                    ImportSetCount++;
                }
            }
        }

        public class ImportOnOverridenPropertyWithSameContract : ImportOnVirtualProperty
        {
            [Import("VirtualImport")]
            public override int VirtualImport
            {
                get
                {
                    return base.VirtualImport;
                }
                set
                {
                    base.VirtualImport = value;
                }
            }
        }

        [TestMethod]
        public void Import_VirtualPropertyOverrideWithSameContract_ShouldSucceed()
        {
            var container = ContainerFactory.Create();
            container.AddAndComposeExportedObject<int>("VirtualImport", 21);

            var import = new ImportOnOverridenPropertyWithSameContract();

            container.SatisfyImports(import);

            // Import will get set twice because there are 2 imports on the same property.
            // We would really like to either elminate it getting set twice or error in this case
            // but we figure it is a rare enough corner case that it doesn't warrented the run time cost
            // and can be covered by an FxCop rule.

            Assert.AreEqual(2, import.ImportSetCount);
            Assert.AreEqual(21, import.VirtualImport);
        }

        public class ImportOnOverridenPropertyWithDifferentContract : ImportOnVirtualProperty
        {
            [Import("OverriddenImport")]
            public override int VirtualImport
            {
                set
                {
                    base.VirtualImport = value;
                }
            }
        }

        [TestMethod]
        public void Import_VirtualPropertyOverrideWithDifferentContract_ShouldSucceed()
        {
            var container = ContainerFactory.Create();
            container.AddAndComposeExportedObject<int>("VirtualImport", 21);
            container.AddAndComposeExportedObject<int>("OverriddenImport", 42);

            var import = new ImportOnOverridenPropertyWithSameContract();

            container.SatisfyImports(import);

            // Import will get set twice because there are 2 imports on the same property.
            // We would really like to either elminate it getting set twice or error in this case
            // but we figure it is a rare enough corner case that it doesn't warrented the run time cost
            // and can be covered by an FxCop rule.

            Assert.AreEqual(2, import.ImportSetCount);

            // The derived most import should be discovered first and so it will get set first
            // and thus the value should be the base import which is 21.
            Assert.AreEqual(21, import.VirtualImport);
        }

        [Export]
        [PartExportsInherited]
        public interface IOrderScreen { }

        public class NorthwindOrderScreen : IOrderScreen
        {
        }

        public class SouthsandOrderScreen : IOrderScreen
        {
        }

        [TestMethod]
        public void Export_ExportOnlyOnBaseInterfacewithInheritedMarked_ShouldFindAllImplementers()
        {
            var container = ContainerFactory.CreateWithAttributedCatalog(
                typeof(NorthwindOrderScreen),
                typeof(SouthsandOrderScreen));

            var exports = container.GetExportedObjects<IOrderScreen>();

            Assert.AreEqual(2, exports.Count);
            Assert.IsInstanceOfType(exports[0], typeof(NorthwindOrderScreen));
            Assert.IsInstanceOfType(exports[1], typeof(SouthsandOrderScreen));
        }

    }
}
