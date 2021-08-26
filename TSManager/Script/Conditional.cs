using System;
using ScratchNet;

namespace TSManager.Script
{
    public class Conditional : Expression
    {
        public Expression Left
        {
            get;
            set;
        }

        public Operator Operator
        {
            get;
            set;
        }

        public Expression Right
        {
            get;
            set;
        }

        public string ValueType
        {
            get;
            set;
        } = "any";


        public string PropertyType
        {
            get;
            set;
        } = "any";


        public override string ReturnType => ValueType;

        public override Descriptor Descriptor
        {
            get
            {
                Descriptor descriptor = new()
                {
                    new ExpressionDescriptor(this, "Left", PropertyType),
                    new TextItemDescriptor(this, " "),
                    new SelectionItemDescriptor(this, "Operator", new object[18]
                {
                    "==",
                    "!=",
                    ">",
                    ">=",
                    "<",
                    "<=",
                    "&&",
                    "||",
                    "+",
                    "-",
                    "*",
                    "/",
                    "%",
                    "&",
                    "|",
                    "^",
                    ">>",
                    "<<"
                }, new object[18]
                {
                    Operator.Equal,
                    Operator.NotEqual,
                    Operator.Great,
                    Operator.GreatOrEqual,
                    Operator.Less,
                    Operator.LessOrEqual,
                    Operator.And,
                    Operator.Or,
                    Operator.Add,
                    Operator.Minus,
                    Operator.Mulitiply,
                    Operator.Divide,
                    Operator.Mod,
                    Operator.BitAnd,
                    Operator.BitOr,
                    Operator.BitExclusiveOr,
                    Operator.BitRightShift,
                    Operator.BitLeftShift
                }),
                    new TextItemDescriptor(this, " "),
                    new ExpressionDescriptor(this, "Right", PropertyType)
                };
                return descriptor;
            }
        }

        public override string Type => "BinaryExpression";

        public bool IsClosing => false;

        public Conditional()
        {
            Operator = Operator.Add;
        }

        private bool IsLogicalOperator(Operator op)
        {
            if (op == Operator.And || op == Operator.Or)
            {
                return true;
            }

            return false;
        }

        public bool IsCompareOperator(Operator op)
        {
            if (op == Operator.Equal || op == Operator.Less || op == Operator.LessOrEqual || op == Operator.Great || op == Operator.GreatOrEqual)
            {
                return true;
            }

            return false;
        }

        private Completion ExecuteLogical(ExecutionEnvironment enviroment, object left, object right)
        {
            if (!(left is bool))
            {
                return Completion.Exception("所给值非 bool 类型", Left);
            }

            if (!(right is bool))
            {
                return Completion.Exception("所给值非 bool 类型", Right);
            }

            try
            {
                bool value = TypeConverters.GetValue<bool>(left);
                bool value2 = TypeConverters.GetValue<bool>(right);
                return Operator switch
                {
                    Operator.And => new Completion(value && value2),
                    Operator.Or => new Completion(value || value2),
                    _ => Completion.Exception("未知错误", this),
                };
            }
            catch (Exception ex)
            {
                return Completion.Exception(ex.Message, this);
            }
        }

        private Completion ExecuteStringAdd(ExecutionEnvironment environment, object left, object right)
        {
            return new Completion(left?.ToString() + right);
        }

        private Completion ExecuteDateMinus(ExecutionEnvironment environment, object left, object right)
        {
            return new Completion((DateTime)left - (DateTime)right);
        }

        protected override Completion ExecuteImpl(ExecutionEnvironment enviroment)
        {
            if (Left == null)
            {
                return Completion.Exception("左侧值无效", this);
            }

            if (Right == null)
            {
                return Completion.Exception("右侧值无效", this);
            }

            Completion completion = Left.Execute(enviroment);
            if (completion.Type != 0)
            {
                return completion;
            }

            Completion completion2 = Right.Execute(enviroment);
            if (completion2.Type != 0)
            {
                return completion2;
            }

            if (IsLogicalOperator(Operator))
            {
                return ExecuteLogical(enviroment, completion.ReturnValue, completion2.ReturnValue);
            }

            if (Operator == Operator.Add && (completion.ReturnValue is string || completion2.ReturnValue is string))
            {
                return ExecuteStringAdd(enviroment, completion.ReturnValue, completion2.ReturnValue);
            }

            if (Operator == Operator.Equal)
            {
                if (completion.ReturnValue is string)
                {
                    return new Completion(completion.ReturnValue is string && (completion.ReturnValue as string).Equals(completion2.ReturnValue));
                }

                if (completion2.ReturnValue is string)
                {
                    return new Completion(completion2.ReturnValue is string && (completion2.ReturnValue as string).Equals(completion.ReturnValue));
                }

                if (completion.ReturnValue is bool boolean)
                {
                    return new Completion(completion2.ReturnValue is bool boolean2 && boolean2 == boolean);
                }

                if (completion2.ReturnValue is bool boolean1)
                {
                    return new Completion(completion.ReturnValue is bool boolean2 && boolean2 == boolean1);
                }

                if (completion.ReturnValue == null)
                {
                    if (completion2.ReturnValue == null)
                    {
                        return new Completion(true);
                    }

                    return new Completion(false);
                }

                if (completion2.ReturnValue == null)
                {
                    if (completion.ReturnValue == null)
                    {
                        return new Completion(true);
                    }

                    return new Completion(false);
                }
            }

            if (Operator == Operator.NotEqual)
            {
                if (completion.ReturnValue is string)
                {
                    return new Completion(!(completion.ReturnValue as string).Equals(completion2.ReturnValue));
                }

                if (completion2.ReturnValue is string)
                {
                    return new Completion(!(completion2.ReturnValue as string).Equals(completion.ReturnValue));
                }

                if (completion.ReturnValue is bool boolean)
                {
                    return new Completion(!(completion2.ReturnValue is bool) || (bool)completion2.ReturnValue != boolean);
                }

                if (completion2.ReturnValue is bool)
                {
                    return new Completion(!(completion.ReturnValue is bool) || (bool)completion.ReturnValue != (bool)completion2.ReturnValue);
                }

                if (completion.ReturnValue == null)
                {
                    if (completion2.ReturnValue != null)
                    {
                        return new Completion(true);
                    }

                    return new Completion(false);
                }

                if (completion2.ReturnValue == null)
                {
                    if (completion.ReturnValue != null)
                    {
                        return new Completion(true);
                    }

                    return new Completion(false);
                }
            }

            if (Operator == Operator.Add && completion.ReturnValue is DateTime && completion2.ReturnValue is DateTime)
            {
                return ExecuteStringAdd(enviroment, completion.ReturnValue, completion2.ReturnValue);
            }

            Type type = TypeConverters.GetMaxTypes(completion.ReturnValue, completion2.ReturnValue);
            if (type == null)
            {
                return Completion.Exception("填入的值非数字", this);
            }

            if (Operator == Operator.Mod || Operator == Operator.BitAnd || Operator == Operator.BitOr || Operator == Operator.BitLeftShift || Operator == Operator.BitRightShift || Operator == Operator.BitExclusiveOr)
            {
                type = typeof(int);
            }

            if (type.Equals(typeof(char)))
            {
                try
                {
                    char value = TypeConverters.GetValue<char>(completion.ReturnValue);
                    char value2 = TypeConverters.GetValue<char>(completion2.ReturnValue);
                    return Operator switch
                    {
                        Operator.Add => new Completion(value + value2),
                        Operator.Minus => new Completion(value - value2),
                        Operator.Mulitiply => new Completion(value * value2),
                        Operator.Divide => new Completion((int)value / (int)value2),
                        Operator.Mod => new Completion((int)value % (int)value2),
                        Operator.Great => new Completion(value > value2),
                        Operator.GreatOrEqual => new Completion(value >= value2),
                        Operator.Less => new Completion(value < value2),
                        Operator.LessOrEqual => new Completion(value <= value2),
                        Operator.Equal => new Completion(value == value2),
                        Operator.NotEqual => new Completion(value != value2),
                        Operator.BitAnd => new Completion(value & value2),
                        Operator.BitOr => new Completion(value | value2),
                        Operator.BitLeftShift => new Completion((int)((uint)value << (int)value2)),
                        Operator.BitRightShift => new Completion((int)value >> (int)value2),
                        Operator.BitExclusiveOr => new Completion(value ^ value2),
                        _ => Completion.Exception("未知错误", this),
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return Completion.Exception(ex.Message, this);
                }
            }

            if (type.Equals(typeof(int)))
            {
                try
                {
                    int value3 = TypeConverters.GetValue<int>(completion.ReturnValue);
                    int value4 = TypeConverters.GetValue<int>(completion2.ReturnValue);
                    return Operator switch
                    {
                        Operator.Add => new Completion(value3 + value4),
                        Operator.Minus => new Completion(value3 - value4),
                        Operator.Mulitiply => new Completion(value3 * value4),
                        Operator.Divide => new Completion(value3 / value4),
                        Operator.Mod => new Completion(value3 % value4),
                        Operator.Great => new Completion(value3 > value4),
                        Operator.GreatOrEqual => new Completion(value3 >= value4),
                        Operator.Less => new Completion(value3 < value4),
                        Operator.LessOrEqual => new Completion(value3 <= value4),
                        Operator.Equal => new Completion(value3 == value4),
                        Operator.NotEqual => new Completion(value3 != value4),
                        Operator.BitAnd => new Completion(value3 & value4),
                        Operator.BitOr => new Completion(value3 | value4),
                        Operator.BitLeftShift => new Completion(value3 << value4),
                        Operator.BitRightShift => new Completion(value3 >> value4),
                        Operator.BitExclusiveOr => new Completion(value3 ^ value4),
                        _ => Completion.Exception("未知错误", this),
                    };
                }
                catch (Exception ex2)
                {
                    Console.WriteLine(ex2.StackTrace);
                    return Completion.Exception(ex2.Message, this);
                }
            }

            if (type.Equals(typeof(float)))
            {
                try
                {
                    float value5 = TypeConverters.GetValue<float>(completion.ReturnValue);
                    float value6 = TypeConverters.GetValue<float>(completion2.ReturnValue);
                    return Operator switch
                    {
                        Operator.Add => new Completion(value5 + value6),
                        Operator.Minus => new Completion(value5 - value6),
                        Operator.Mulitiply => new Completion(value5 * value6),
                        Operator.Divide => new Completion(value5 / value6),
                        Operator.Great => new Completion(value5 > value6),
                        Operator.GreatOrEqual => new Completion(value5 >= value6),
                        Operator.Less => new Completion(value5 < value6),
                        Operator.LessOrEqual => new Completion(value5 <= value6),
                        Operator.Equal => new Completion(value5 == value6),
                        Operator.NotEqual => new Completion(value5 != value6),
                        _ => Completion.Exception("未知错误", this),
                    };
                }
                catch (Exception ex3)
                {
                    return Completion.Exception(ex3.Message, this);
                }
            }

            if (type.Equals(typeof(long)))
            {
                try
                {
                    long value7 = TypeConverters.GetValue<long>(completion.ReturnValue);
                    long value8 = TypeConverters.GetValue<long>(completion2.ReturnValue);
                    return Operator switch
                    {
                        Operator.Add => new Completion(value7 + value8),
                        Operator.Minus => new Completion(value7 - value8),
                        Operator.Mulitiply => new Completion(value7 * value8),
                        Operator.Divide => new Completion(value7 / value8),
                        Operator.Great => new Completion(value7 > value8),
                        Operator.GreatOrEqual => new Completion(value7 >= value8),
                        Operator.Less => new Completion(value7 < value8),
                        Operator.LessOrEqual => new Completion(value7 <= value8),
                        Operator.Equal => new Completion(value7 == value8),
                        Operator.NotEqual => new Completion(value7 != value8),
                        _ => Completion.Exception("未知错误", this),
                    };
                }
                catch (Exception ex4)
                {
                    return Completion.Exception(ex4.Message, this);
                }
            }

            try
            {
                double value9 = TypeConverters.GetValue<double>(completion.ReturnValue);
                double value10 = TypeConverters.GetValue<double>(completion2.ReturnValue);
                return Operator switch
                {
                    Operator.Add => new Completion(value9 + value10),
                    Operator.Minus => new Completion(value9 - value10),
                    Operator.Mulitiply => new Completion(value9 * value10),
                    Operator.Divide => new Completion(value9 / value10),
                    Operator.Great => new Completion(value9 > value10),
                    Operator.GreatOrEqual => new Completion(value9 >= value10),
                    Operator.Less => new Completion(value9 < value10),
                    Operator.LessOrEqual => new Completion(value9 <= value10),
                    Operator.Equal => new Completion(value9 == value10),
                    Operator.NotEqual => new Completion(value9 != value10),
                    _ => Completion.Exception("未知错误", this),
                };
            }
            catch (Exception ex5)
            {
                return Completion.Exception(ex5.Message, this);
            }
        }
    }
}