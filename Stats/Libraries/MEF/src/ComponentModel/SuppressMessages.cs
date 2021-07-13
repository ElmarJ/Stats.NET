// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope = "namespace", Target = "System.ComponentModel.Composition",
                        Justification = "Approved by Framework")]

[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "System.ComponentModel.Composition.Caching",
                        Justification = "Approved by Framework")]

[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "System.ComponentModel.Composition.Hosting")]

[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "System.ComponentModel.Composition.Primitives")]
[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "System.ComponentModel.Composition.ReflectionModel")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.ComponentModel.Composition.ReflectionModel")]

// BUG: DDB 90145 - GenericMethodsShouldProvideTypeParameter should ignore methods that returns T (Code Analysis bug)
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExports`2(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExports`2(System.Linq.Expressions.Expression`1<System.Func`2<System.ComponentModel.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExports`2()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExports`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExports`1(System.Linq.Expressions.Expression`1<System.Func`2<System.ComponentModel.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExports`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExportedObjects`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExportedObjects`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExportedObjectOrDefault`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExportedObjectOrDefault`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExportedObject`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExportedObject`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExport`2(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExport`2(System.Linq.Expressions.Expression`1<System.Func`2<System.ComponentModel.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",  Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExport`2(System.Linq.Expressions.Expression`1<System.Func`2<System.ComponentModel.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExport`2()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExport`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExport`1(System.Linq.Expressions.Expression`1<System.Func`2<System.ComponentModel.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.ExportProvider.#GetExport`1()")]

#if SILVERLIGHT

// BUG: Dev10 - 434542 ImplementStandardExceptionConstructors fires on Silverlight exceptions even though serialization is not supported on that Framework (Code Analysis bug)
[module: SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Scope = "type", Target = "System.ComponentModel.Composition.CompositionException")]
[module: SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Scope = "type", Target = "System.ComponentModel.Composition.CardinalityMismatchException")]
[module: SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Scope = "type", Target = "Microsoft.Internal.Assumes+InternalErrorException")]

// Code Analysis bugs
[module: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope = "member", Target = "System.ComponentModel.Composition.CompositionException.#_message")]
[module: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope = "member", Target = "System.ComponentModel.Composition.ComposablePartException.#_id")]
[module: SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Scope="type", Target="System.ComponentModel.Composition.ICompositionError")]
[module: SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Scope="type", Target="System.ComponentModel.Composition.IAttributedImport")]
[module: SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Scope="type", Target="System.ComponentModel.Composition.ReflectionModel.IReflectionPartCreationInfo")]
#endif

// All of these will go away when more types in Advanced and Primitives, and Tuple is removed from System.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Threading")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.ComponentModel.Composition.Primitives")]

// These warnings are deliberate design decision. ICompositionElement is an advanced type and we don't want to dirty the API by make these members public
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.AssemblyCatalog.#System.ComponentModel.Composition.Primitives.ICompositionElement.DisplayName")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.AssemblyCatalog.#System.ComponentModel.Composition.Primitives.ICompositionElement.Origin")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.DirectoryCatalog.#System.ComponentModel.Composition.Primitives.ICompositionElement.DisplayName")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.DirectoryCatalog.#System.ComponentModel.Composition.Primitives.ICompositionElement.Origin")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.TypeCatalog.#System.ComponentModel.Composition.Primitives.ICompositionElement.DisplayName")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.ComponentModel.Composition.Hosting.TypeCatalog.#System.ComponentModel.Composition.Primitives.ICompositionElement.Origin")]
