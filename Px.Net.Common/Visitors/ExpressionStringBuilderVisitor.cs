using System;
using System.Linq.Expressions;
using System.Text;

namespace Px.Net.Common.Visitors
{
    public class ExpressionStringBuilderVisitor : ExpressionVisitor
    {
        private readonly StringBuilder _builder = new();

        public override string ToString() => _builder.ToString();

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    _builder.Append('!');
                    Visit(node.Operand);
                    break;
                default:
                    throw new NotSupportedException($"Unary operator '{node.NodeType}' is not supported.");
            }
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _builder.Append('(');
            Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _builder.Append(" && ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _builder.Append(" || ");
                    break;
                case ExpressionType.Equal:
                    _builder.Append(" == ");
                    break;
                case ExpressionType.NotEqual:
                    _builder.Append(" != ");
                    break;
                case ExpressionType.GreaterThan:
                    _builder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _builder.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    _builder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _builder.Append(" <= ");
                    break;
                default:
                    throw new NotSupportedException($"Binary operator '{node.NodeType}' is not supported.");
            }

            Visit(node.Right);
            _builder.Append(") ");

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                _builder.Append("null");
            }
            else
            {
                var value = node.Value.ToString();
                if (node.Value is string)
                {
                    value = "\"" + value + "\"";
                }
                _builder.Append(value);
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _builder.Append(node.Member.Name);
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            //_builder.Append(node.Name);
            return node;
        }
    }
}

