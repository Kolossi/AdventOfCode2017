using OneOf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    // see 2016 day 12!
    class Day18 : Day
    {
        private const string _SOUND_ = "_snd";
        private const string _RECEIVE_ = "_rcv";
        public class Token : OneOfBase<Token.Register, Token.LongValue>
        {
            public static Token GetToken(string input)
            {
                long val;
                if (long.TryParse(input, out val))
                {
                    return new LongValue() { Value = val };
                }
                else
                {
                    return new Register() { RegName = input };
                }
            }

            public class Register : Token
            {
                public string RegName { get; set; }

                public override string ToString()
                {
                    return RegName;
                }
            }

            public class LongValue : Token
            {
                public long Value { get; set; }

                public override string ToString()
                {
                    return Value.ToString();
                }
            }

            public override string ToString()
            {
                return this.Match(reg => reg.ToString(), val => val.ToString());
            }
        }

        public class Instruction : OneOfBase<Instruction.Copy, Instruction.Op, Instruction.Receive, Instruction.Jump>
        {
            public static Instruction GetInstruction(string input)
            {
                var parts = GetParts(input);
                Instruction instruction;
                switch (parts[0])
                {
                    case "snd":
                        instruction = new Instruction.Copy()
                        {
                            Source = Token.GetToken(parts[1]),
                            Destination = new Token.Register() { RegName = _SOUND_ }
                        };
                        break;
                    case "set":
                        instruction = new Instruction.Copy()
                        {
                            Source = Token.GetToken(parts[2]),
                            Destination = new Token.Register() { RegName = parts[1] }
                        };
                        break;
                    case "add":
                        instruction =
                            new Instruction.Op()
                            {
                                Register = new Token.Register() { RegName = parts[1] },
                                Value = Token.GetToken(parts[2]),
                                Operation = (a, b) => a + b
                            };
                        break;
                    case "mul":
                        instruction =
                            new Instruction.Op()
                            {
                                Register = new Token.Register() { RegName = parts[1] },
                                Value = Token.GetToken(parts[2]),
                                Operation = (a, b) => a * b
                            };
                        break;

                    case "mod":
                        instruction =
                            new Instruction.Op()
                            {
                                Register = new Token.Register() { RegName = parts[1] },
                                Value = Token.GetToken(parts[2]),
                                Operation = (a, b) => a % b
                            };
                        break;

                    case "rcv":
                        instruction =
                            new Instruction.Receive()
                            {
                                Compare = Token.GetToken(parts[1]),
                            };
                        break;

                    case "jgz":
                        instruction = new Instruction.Jump()
                        {
                            Compare = Token.GetToken(parts[1]),
                            Offset = Token.GetToken(parts[2])
                        };
                        break;
                    default:
                        throw new InvalidOperationException("Assembunny");
                }
                return instruction;
            }

            public class Copy : Instruction
            {
                public Token Source { get; set; }
                public Token.Register Destination { get; set; }

                public void ExecuteCopy(Alu alu)
                {
                    alu.SetRegValue(Destination,
                        Source.Match(reg => alu.GetRegValue(reg),
                                     val => val.Value));
                }
            }

            public class Op : Instruction
            {
                public Token.Register Register { get; set; }
                public Token Value { get; set; }
                public Func<long, long, long> Operation { get; set; }

                public void ExecuteOp(Alu alu)
                {
                    alu.SetRegValue(Register, Operation(alu.GetRegValue(Register),
                        Value.Match(reg => alu.GetRegValue(reg),
                                     val => val.Value)));
                }
            }

            public class Receive : Instruction
            {
                public Token Compare { get; set; }
                public Token.Register Rcv = new Token.Register { RegName = _RECEIVE_ };
                public Token.Register Snd = new Token.Register { RegName = _SOUND_ };

                public void ExecuteReceive(Alu alu)
                {
                    var value = Compare.Match(reg => alu.GetRegValue(reg), val => val.Value);
                    if (value != 0)
                    {
                        alu.SetRegValue(Rcv, alu.GetRegValue(Snd));
                    }
                }
            }

            public class Jump : Instruction
            {
                public Token Compare { get; set; }
                public Token Offset { get; set; }

                public void ExecuteJump(Alu alu)
                {
                    if (Compare.Match(reg => alu.GetRegValue(reg), val => val.Value) > 0)
                    {
                        var offsetVal = Offset.Match(reg => alu.GetRegValue(reg), val => val.Value);
                        if (offsetVal == 0) throw new StackOverflowException("Assembunny");
                        alu.AdjustPtr(offsetVal - 1);
                    }
                }
            }

            public void Execute(Alu alu)
            {
                this.Switch(copy => copy.ExecuteCopy(alu),
                    op => op.ExecuteOp(alu),
                    recv => recv.ExecuteReceive(alu),
                    jump => jump.ExecuteJump(alu));
            }

        }

        public class Alu
        {
            private Instruction[] Instructions { get; set; }
            public Dictionary<string, long> Registers { get; set; }
            public long Ptr { get; set; }

            public Alu(string input)
            {
                Registers = new Dictionary<string, long>();
                Ptr = 0;
                var lines = GetLines(input);
                Instructions = new Instruction[lines.Length];
                foreach (var line in lines)
                {
                    Instructions[Ptr++] = Instruction.GetInstruction(line);
                }
                Ptr = 0;

            }

            public long GetRegValue(Token.Register reg)
            {
                if (!Registers.ContainsKey(reg.RegName))
                {
                    Registers[reg.RegName] = 0;
                    return 0;
                }
                return Registers[reg.RegName];
            }

            public void SetRegValue(Token.Register reg, long value)
            {
                Registers[reg.RegName] = value;
            }

            public void AdjustPtr(long offset)
            {
                Ptr += offset;
            }

            public void ExecuteProgram(bool haltOnReceive = false)
            {
                Ptr = 0;
                while (Ptr >= 0 && Ptr < Instructions.Length && (!haltOnReceive || !Registers.ContainsKey(_RECEIVE_)))
                {
                    Instructions[Ptr].Execute(this);
                    Ptr++;
                }
            }
        }

        public override string First(string input)
        {
            //7481 too low
            var alu = new Alu(input);
            alu.ExecuteProgram(haltOnReceive: true);
            return alu.Registers[_RECEIVE_].ToString();
        }

        public override string Second(string input)
        {
            var alu = new Alu(input);
            alu.Registers["c"] = 1;
            alu.ExecuteProgram();
            return alu.Registers["a"].ToString();
        }

    }
}
