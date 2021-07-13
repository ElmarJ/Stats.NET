// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Text;
using Microsoft.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.UnitTesting;
using Tests.Integration;
using System.Reflection;
using System.Reflection.Emit;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionServicesTests
    {
        private static readonly Type NonGenericType = typeof(int);

        [TestMethod]
        public void GetContractName_NullAsTypeArgument_ThrowsArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("type", () =>
                {
                    AttributedModelServices.GetContractName((Type)null);
                });
        }        

        [TestMethod]
        [Description("Verifies that AttributedModelServices.GetContractName retrieves ContractType.Name property from a type decorated with ContractType attribute.")]
        public void GetContractNameTestContractType()
        {
            Type type = typeof(TestContractType);
            string expected = TestContractType.TestContractName;
            string actual = AttributedModelServices.GetContractName(type);
            Assert.AreEqual<string>(expected, actual, "Contract name of a type decorated with ContractType attribute should be ContractType.Name property.");

            string typeIdentity = AttributedModelServices.GetTypeIdentity(type);
            Assert.AreNotEqual(expected, typeIdentity, "Type identity should not take into account ContractTypeAttribute");
        }

        [TestMethod]
        [Description("Verifying that ContractTypeAttribute is not inheritable.")]
        public void TestTypeDerivingFromTestContractType()
        {
            Type type = typeof(TypeDerivingFromTestContractType);
            string expected = ContractNameServices.GetDefaultContractName(type);
            string actual = AttributedModelServices.GetContractName(type);
            Assert.AreEqual<string>(expected, actual, "ContractTypeAttribute is not inheritable so the contract name for type " +
            "deriving from type decoracted with ContractTypeAttribute should be the same as default contract name.");
        }

        [TestMethod]
        [Description("Verifying that when ContractTypeAttribute has ContractName equal to null, default contract name is used.")]
        public void TestTypeWithContractTypeWithoutName()
        {
            Type type = typeof(TestContractTypeWithoutName);
            string expected = ContractNameServices.GetDefaultContractName(type);
            string actual = AttributedModelServices.GetContractName(type);
            Assert.AreEqual<string>(expected, actual, " When ContractTypeAttribute has ContractName equal to null, default contract name should be used.");
        }

        [TestMethod]
        [Description("Verifying that a type  exporting a type decorated with ContractType attribute should have ExportName equal to ContractType.Name. " +
            "Same should apply to descendants of this type.")]
        public void TestExportTypeWithContractTypeAttribute()
        {
            Type type = typeof(ExportedTypeWithContractType);
            string expected = TestContractType.TestContractName;
            string actual = type.GetContractNameFromExport(type.GetFirstAttribute<ExportAttribute>());
            Assert.AreEqual<string>(expected, actual,
            "A type exporting a type decorated with ContractType attribute should have ExportName equal to ContractType.Name");

            type = typeof(TypeDerivingFromExportedTypeWithContractType);
            actual = type.GetContractNameFromExport(type.BaseType.GetFirstAttribute<ExportAttribute>());
            Assert.AreEqual<string>(expected, actual,
            "A type deriving from type exporting a type decorated with ContractType attribute should have ExportName equal to ContractType.Name");
        }

        [TestMethod]
        [Description("Verifies adding custom modifiers to contract name.")]
        public void ContractNameServicesAddCustomModifiersTest()
        {
            Type[] modifiers = new Type[] { typeof(int), typeof(List<int>), typeof(double) };
            StringBuilder typeName = new StringBuilder();
            ContractNameServices.WriteCustomModifiers(typeName, "test", modifiers);
            Assert.AreEqual<string>(
                string.Format(" {0}(System.Int32,System.Collections.Generic.List(System.Int32),System.Double)", "test"),
                typeName.ToString(),
                "Expecting correct modifiers text.");
        }

        [TestMethod]
        [Description("Verifies CompositionServices.GetDefaultContractName method.")]
        public void GetDefaultContractNameTest()
        {
            ExpectationCollection<Type, string> expectations = new ExpectationCollection<Type, string>();

            expectations.Add(typeof(string), "System.String");
            expectations.Add(typeof(int?), "System.Nullable(System.Int32)");
            expectations.Add(typeof(NestedParent.NestedChild), "System.ComponentModel.Composition.NestedParent+NestedChild");
            expectations.Add(typeof(int[,]), "System.Int32[,]");
            expectations.Add(typeof(int[,][][]), "System.Int32[,][][]");
            expectations.Add(typeof(Array[,][][]).MakePointerType(), "System.Array[,][][]*");
            expectations.Add(typeof(int).MakePointerType(), "System.Int32*");
            expectations.Add(typeof(int).MakePointerType().MakeArrayType(3).MakeArrayType().MakePointerType(), "System.Int32*[][,,]*");
            expectations.Add(typeof(int).MakeByRefType(), "System.Int32&");
            expectations.Add(typeof(int).MakePointerType().MakeArrayType(4).MakeArrayType().MakeByRefType(), "System.Int32*[][,,,]&");
            expectations.Add(typeof(List<>), "System.Collections.Generic.List()");
            expectations.Add(typeof(Dictionary<int,double>), "System.Collections.Generic.Dictionary(System.Int32,System.Double)");
            expectations.Add(typeof(Dictionary<Dictionary<int,double>,double>), "System.Collections.Generic.Dictionary(System.Collections.Generic.Dictionary(System.Int32,System.Double),System.Double)");
            expectations.Add(typeof(GenericContract1<int>.GenericContract2.GenericContract3<double>), "System.ComponentModel.Composition.GenericContract1(System.Int32)+GenericContract2+GenericContract3(System.Double)");
            expectations.Add(typeof(GenericContract4<int,double>.GenericContract5<double,int>.GenericContract6<int,double>), "System.ComponentModel.Composition.GenericContract4(System.Int32,System.Double)+GenericContract5(System.Double,System.Int32)+GenericContract6(System.Int32,System.Double)");
            expectations.Add(typeof(GenericContract4<,>.GenericContract5<,>.GenericContract6<,>), "System.ComponentModel.Composition.GenericContract4(,)+GenericContract5(,)+GenericContract6(,)");
            expectations.Add(typeof(GenericContract7), "System.ComponentModel.Composition.GenericContract7");
            expectations.Add(typeof(GenericContract8<int>), "System.ComponentModel.Composition.GenericContract8(System.Int32)");
            expectations.Add(typeof(GenericContract4<int,double>.GenericContract5<double,int>[][,,,,]), "System.ComponentModel.Composition.GenericContract4(System.Int32,System.Double)+GenericContract5(System.Double,System.Int32)[][,,,,]");
            expectations.Add(typeof(GenericContract4<int,double>.GenericContract5<double,int>).MakePointerType(), "System.ComponentModel.Composition.GenericContract4(System.Int32,System.Double)+GenericContract5(System.Double,System.Int32)*");
            expectations.Add(typeof(GenericContract4<int,double>.GenericContract5<double,int>[][,]).MakePointerType().MakeArrayType(4), "System.ComponentModel.Composition.GenericContract4(System.Int32,System.Double)+GenericContract5(System.Double,System.Int32)[][,]*[,,,]");            expectations.Add(typeof(OuterClassWithGenericNested.GenericNested<int>), "System.ComponentModel.Composition.OuterClassWithGenericNested+GenericNested(System.Int32)");
            expectations.Add(typeof(Delegate), "System.Delegate");
            expectations.Add(typeof(MulticastDelegate), "System.MulticastDelegate");
            expectations.Add(typeof(Action), "System.Void()");
            expectations.Add(typeof(Func<Func<string>>), "System.Func(System.String)()");
            expectations.Add(typeof(DelegateCompositionTests.DoWorkDelegate), "System.Object(System.Int32,System.Object&,System.String&)");
            expectations.Add(typeof(DelegateCompositionTests.DelegateOneArg), "System.Int32(System.Int32)");
            expectations.Add(typeof(DelegateCompositionTests.DelegateTwoArgs), "System.Int32(System.Int32,System.Int32)");
            expectations.Add(typeof(DelegateCompositionTests.SimpleDelegate), "System.Int32()");
            expectations.Add(typeof(DelegateCompositionTests.ActionWith9Args), "System.Void(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)");
            expectations.Add(typeof(DelegateCompositionTests.FuncWith9Args), "System.Int32(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)");
            expectations.Add(EmitGenericType("MyGeneratedType", typeof(int)), "MyGeneratedType(System.Int32)");
            expectations.Add(EmitGenericType("MyGeneratedType`1", typeof(string)), "MyGeneratedType(System.String)");
            expectations.Add(EmitGenericType("_name_", typeof(List<>)), "_name_(System.Collections.Generic.List())");

            // This particular case can clash with the name generation for methods because we use () for both generics and methods
            // we may want to fix this clash by changing the generic markers but it is likely to be very rare.
            expectations.Add(EmitGenericType("System.Void", typeof(int)), "System.Void(System.Int32)");

            foreach (var e in expectations)
            {
                string typeIdentity = AttributedModelServices.GetTypeIdentity(e.Input);
                Assert.AreEqual(e.Output, typeIdentity);
            }

            // Do it again to excerise the cache.
            foreach (var e in expectations)
            {
                string typeIdentity = AttributedModelServices.GetTypeIdentity(e.Input);
                Assert.AreEqual(e.Output, typeIdentity);
            }
        }

        public Type EmitGenericType(string typeName, Type genericArgumentType)
        {
            var assemblyName = new AssemblyName("EmittedTestAssembly");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("EmittedTestModule");
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);

            GenericTypeParameterBuilder T = typeBuilder.DefineGenericParameters(new string[] { "T" })[0];
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExportAttribute).GetConstructor(Type.EmptyTypes), new object[0]));

            return typeBuilder.CreateType().MakeGenericType(genericArgumentType);
        }
    }
}
