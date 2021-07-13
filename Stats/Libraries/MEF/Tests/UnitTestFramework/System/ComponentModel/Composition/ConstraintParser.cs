// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    public class ContraintParser
    {
        private static readonly PropertyInfo _exportDefinitionContractNameProperty = typeof(ExportDefinition).GetProperty("ContractName");
        private static readonly PropertyInfo _exportDefinitionMetadataProperty = typeof(ExportDefinition).GetProperty("Metadata");
        private static readonly MethodInfo _metadataContainsKeyMethod = typeof(IDictionary<string, object>).GetMethod("ContainsKey");

        public static bool TryParseConstraint(Expression<Func<ExportDefinition, bool>> constraint, out string contractName, out IEnumerable<string> requiredMetadata)
        {
            contractName = null;
            requiredMetadata = null;

            List<string> requiredMetadataList = new List<string>();
            foreach (Expression expression in SplitConstraintBody(constraint.Body))
            {
                // First try to parse as a contract, if we don't have one already
                if (contractName == null && TryParseExpressionAsContractConstraintBody(expression, constraint.Parameters[0], out contractName))
                {
                    continue;
                }

                // Then try to parse as a required metadata item name
                string requiredMetadataItemName = null;
                if (TryParseExpressionAsMetadataConstraintBody(expression, constraint.Parameters[0], out requiredMetadataItemName))
                {
                    requiredMetadataList.Add(requiredMetadataItemName);
                    continue;
                }

                // At this point we don't know what this is, so we need to return false - the parsing failed
                contractName = null;
                return false;
            }

            // ContractName should have been set already, just need to set metadata
            requiredMetadata = requiredMetadataList;
            return true;
        }


        private static IEnumerable<Expression> SplitConstraintBody(Expression expression)
        {
            Assert.IsNotNull(expression);

            // The expression we know about should be a set of nested AndAlso's, we
            // need to flatten them into one list. we do this iteratively, as 
            // recursion will create too much of a memory churn.
            Stack<Expression> expressions = new Stack<Expression>();
            expressions.Push(expression);

            while (expressions.Count > 0)
            {
                Expression current = expressions.Pop();
                if (current.NodeType == ExpressionType.AndAlso)
                {
                    BinaryExpression andAlso = (BinaryExpression)current;
                    // Push right first - this preserves the ordering of the expression, which will force
                    // the contract constraint to come up first as the callers are optimized for this form
                    expressions.Push(andAlso.Right);
                    expressions.Push(andAlso.Left);
                    continue;
                }

                yield return current;
            }
        }

        private static bool TryParseExpressionAsContractConstraintBody(Expression expression, Expression parameter, out string contractName)
        {
            contractName = null;

            // The expression should be an '==' expression
            if (expression.NodeType != ExpressionType.Equal)
            {
                return false;
            }

            BinaryExpression contractConstraintExpression = (BinaryExpression)expression;

            // First try item.ContractName == "Value"
            if (TryParseContractNameFromEqualsExpression(contractConstraintExpression.Left, contractConstraintExpression.Right, parameter, out contractName))
            {
                return true;
            }

            // Then try "Value == item.ContractName
            if (TryParseContractNameFromEqualsExpression(contractConstraintExpression.Right, contractConstraintExpression.Left, parameter, out contractName))
            {
                return true;
            }

            return false;
        }

        private static bool TryParseContractNameFromEqualsExpression(Expression left, Expression right, Expression parameter, out string contractName)
        {
            contractName = null;

            // The left should be access to property "Contract" applied to the parameter
            MemberExpression targetMember = left as MemberExpression;
            if (targetMember == null)
            {
                return false;
            }

            if ((targetMember.Member != _exportDefinitionContractNameProperty) || (targetMember.Expression != parameter))
            {
                return false;
            }

            // Right should a constant expression containing the contract name
            ConstantExpression contractNameConstant = right as ConstantExpression;
            if (contractNameConstant == null)
            {
                return false;
            }

            if (!TryParseStringConstant(contractNameConstant, out contractName))
            {
                return false;
            }

            return true;
        }

        private static bool TryParseExpressionAsMetadataConstraintBody(Expression expression, Expression parameter, out string requiredMetadataName)
        {
            Assumes.NotNull(expression, parameter);

            requiredMetadataName = null;

            // Should be a call to IDictionary.ContainsKey() on "Metadata" property applied to the parameter            
            MethodCallExpression methodCall = expression as MethodCallExpression;
            if (methodCall == null)
            {
                return false;
            }

            // Make sure that the right method ie being called
            if (methodCall.Method != _metadataContainsKeyMethod)
            {
                return false;
            }

            // Make sure the method is being called on the right object            
            MemberExpression targetMember = methodCall.Object as MemberExpression;
            if (targetMember == null)
            {
                return false;
            }

            if ((targetMember.Expression != parameter) || (targetMember.Member != _exportDefinitionMetadataProperty))
            {
                return false;
            }

            // There should only ever be one argument; otherwise, 
            // we've got the wrong IDictionary.ContainsKey method.
            Assumes.IsTrue(methodCall.Arguments.Count == 1);

            // Argument should a constant expression containing the metadata key
            ConstantExpression requiredMetadataConstant = methodCall.Arguments[0] as ConstantExpression;
            if (requiredMetadataConstant == null)
            {
                return false;
            }

            if (!TryParseStringConstant(requiredMetadataConstant, out requiredMetadataName))
            {
                return false;
            }

            return true;
        }

        private static bool TryParseStringConstant(ConstantExpression constant, out string result)
        {
            Assumes.NotNull(constant);

            if (constant.Type == typeof(string) && constant.Value != null)
            {
                result = (string)constant.Value;
                return true;
            }

            result = null;
            return false;
        }
    }
}
