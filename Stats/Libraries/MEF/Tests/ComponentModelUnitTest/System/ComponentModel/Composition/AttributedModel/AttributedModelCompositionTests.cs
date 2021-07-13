// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.ComponentModel.Composition.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.UnitTesting;

namespace System.ComponentModel.Composition.AttributedModel
{
    [TestClass]
    public class AttributedModelCompositionTests
    {
        [TestMethod]
        public void PartContainingOnlyStaticExports_ShouldNotCauseInstanceToBeCreated()
        {  
            var container = ContainerFactory.CreateWithAttributedCatalog(typeof(HasOnlyStaticExports));
            
            Assert.AreEqual("Field", container.GetExportedObject<string>("Field"));
            Assert.AreEqual("Property", container.GetExportedObject<string>("Property"));
            Assert.IsNotNull("Method", container.GetExportedObject<Func<string>>("Method")());

            Assert.IsFalse(HasOnlyStaticExports.InstanceCreated);
        }

        [TestMethod]
        public void ExportOnAbstractBase_DoesNotReturnNull()
        {   // 499393 - Classes inheriting from an exported 
            // abstract class are exported as 'null'

            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            var definition = PartDefinitionFactory.CreateAttributed(typeof(Derived));
            batch.AddPart(definition.CreatePart());
            container.Compose(batch);

            Assert.IsNotNull(container.GetExportedObjectOrDefault<Base>());
        }

        [TestMethod]
        public void ReadOnlyFieldImport_ShouldThrowComposition()
        {
            var importer = PartFactory.CreateAttributed(new ReadOnlyPropertyImport());
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddExportedObject("readonly", "new value");

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotActivate,
                                          ErrorId.ReflectionModel_ImportNotWritable,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ReadOnlyPropertyImport_ShouldThrowComposition()
        {
            var importer = PartFactory.CreateAttributed(new ReadOnlyPropertyImport());
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            batch.AddExportedObject("readonly", "new value");

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotActivate,
                                          ErrorId.ReflectionModel_ImportNotWritable, RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void WriteOnlyPropertyExport_ShouldThrowComposition()
        {
            var importer = PartFactory.CreateAttributed(new WriteOnlyPropertyExport());
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(importer);
            container.Compose(batch);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotGetExportedObject, ErrorId.ReflectionModel_ExportNotReadable, () =>
            {
                container.GetExportedObject<string>("writeonly");
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromInt32ToDateTime()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("DateTime", typeof(DateTime), 42);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromInt16ToInt32()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("Int32", typeof(Int32), (short)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                              ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                              RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });

        }

        [TestMethod]
        public void ImportValueMismatchFromInt32ToInt16()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("Int16", typeof(Int16), (int)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromInt32ToUInt32()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("UInt32", typeof(UInt32), (int)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromUInt32ToInt32()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("Int32", typeof(Int32), (uint)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromInt32ToInt64()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("Int64", typeof(Int64), (int)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromSingleToDouble()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("Double", typeof(Double), (float)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromDoubleToSingle()
        {
            var container = ContainerFactory.Create(); 
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("Single", typeof(Single), (double)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromSingleToInt32()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("Int32", typeof(Int32), (float)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void ImportValueMismatchFromInt32ToSingle()
        {
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();

            batch.AddPart(new ImportValueTypes());
            batch.AddExportedObject("Single", typeof(Single), (int)10);

            CompositionAssert.ThrowsError(ErrorId.CompositionEngine_PartCannotSetImport,
                                          ErrorId.ReflectionModel_ImportNotAssignableFromExport,
                                          RetryMode.DoNotRetry, () =>
            {
                container.Compose(batch);
            });
        }

        [TestMethod]
        public void MemberExports()
        {
            var exporter = PartFactory.CreateAttributed(new ObjectWithMemberExports());
            var container = ContainerFactory.Create();
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(exporter);
            container.Compose(batch);

            var filedExports = container.GetExports<int>("field");
            ExportsAssert.AreEqual(filedExports, 1, 5);

            var readonlyExports = container.GetExports<int>("readonly");
            ExportsAssert.AreEqual(readonlyExports, 2, 6);

            var propertyExports = container.GetExports<int>("property");
            ExportsAssert.AreEqual(propertyExports, 3, 7);

            var methodExports = container.GetExportedObjects<Func<int, int>>("func").Select(f => f(0));
            EnumerableAssert.AreEqual(methodExports, 4, 8);
        }

        public class ReadOnlyFieldImport
        {
            [Import("readonly")]
            public readonly string Value = "Value";
        }

        public class ReadOnlyPropertyImport
        {
            [Import("readonly")]
            public string Value
            {
                get { return "Value"; }
            }
        }

        public class WriteOnlyPropertyExport
        {
            [Export("writeonly")]
            public string Value
            {
                set { }
            }
        }

        public class ObjectWithMemberExports
        {
            [Export("field")]
            public static int staticField = 1;

            [Export("readonly")]
            public static readonly int staticReadonly = 2;

            [Export("property")]
            public static int StaticProperty { get { return 3; } }

            [Export("func")]
            public static int StaticMethod(int arg1) { return 4; }

            [Export("field")]
            public int instanceField = 5;

            [Export("readonly")]
            public readonly int instanceReadonly = 6;

            [Export("property")]
            public int InstanceProperty { get { return 7; } }

            [Export("func")]
            public int InstanceMethod(int arg1) { return 8; }
        }

        public class HasOnlyStaticExports
        {
            public static bool InstanceCreated;

            public HasOnlyStaticExports()
            {
                InstanceCreated = true;
            }

            [Export("Field")]
            public static string Field = "Field";

            [Export("Property")]
            public static string Property
            {
                get { return "Property"; }
            }

            [Export("Method")]
            public static string Method()
            {
                return "Method";
            }
        }

        [Export(typeof(Base))]
        [PartExportsInherited]
        public abstract class Base
        {
        }

        [Export(typeof(Derived))]
        public class Derived : Base
        {
        }

        public class ImportValueTypes
        {
            [Import("Int16", AllowDefault = true)]
            public short Int16
            {
                get;
                set;
            }

            [Import("Int32", AllowDefault = true)]
            public int Int32
            {
                get;
                set;
            }

            [Import("UInt32", AllowDefault = true)]
            [CLSCompliant(false)]
            public uint UInt32
            {
                get;
                set;
            }

            [Import("Int64", AllowDefault = true)]
            public long Int64
            {
                get;
                set;
            }

            [Import("Single", AllowDefault = true)]
            public float Single
            {
                get;
                set;
            }

            [Import("Double", AllowDefault = true)]
            public double Double
            {
                get;
                set;
            }

            [Import("DateTime", AllowDefault = true)]
            public DateTime DateTime
            {
                get;
                set;
            }
        }
    }
}
