using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    // see 2016 day 12!
    class Day18 : Day
    {
        public static Token.Register _WAITING = new Token.Register() { RegName = "_lck" };
        public static Token.Register _SEND = new Token.Register() { RegName = "_snd" };
        public static Token.Register _RECEIVE = new Token.Register() { RegName = "_rcv" };
        public static Token.Register _FINISHED = new Token.Register() { RegName = "_fin" };

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

        public class Instruction : OneOfBase<Instruction.Copy, Instruction.Op, Instruction.Recover, Instruction.Jump, Instruction.Receive>
        {
            public static Instruction GetInstruction(string input, int firmwareVersion = 1)
            {
                var parts = GetParts(input);
                Instruction instruction;
                switch (parts[0])
                {
                    case "snd":
                        instruction = new Instruction.Copy()
                        {
                            Source = Token.GetToken(parts[1]),
                            Destination = _SEND
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
                        if (firmwareVersion == 2)
                        {
                            instruction =
                                new Instruction.Receive()
                                {
                                    Register = new Token.Register() { RegName = parts[1]} 
                                };
                        }
                        else
                        {
                            instruction =
                                new Instruction.Recover()
                                {
                                    Compare = Token.GetToken(parts[1]),
                                };
                        }
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

            public class Recover : Instruction
            {
                public Token Compare { get; set; }

                public void ExecuteRecover(Alu alu)
                {
                    var value = Compare.Match(reg => alu.GetRegValue(reg), val => val.Value);
                    if (value != 0)
                    {
                        alu.SetRegValue(_RECEIVE, alu.GetRegValue(_SEND));
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

            public class Receive : Instruction
            {
                public Token.Register Register { get; set; }

                public void ExecuteReceive(Alu alu)
                {
                    if (alu.HasRegValue(_RECEIVE))
                    {
                        alu.SetRegValue(Register, alu.GetRegValue(_RECEIVE));
                        alu.RemoveRegValue(_RECEIVE);
                    }
                    else
                    {
                        alu.SetRegValue(_WAITING,1);
                    }
                }
            }

            public void Execute(Alu alu)
            {
                this.Switch(copy => copy.ExecuteCopy(alu),
                    op => op.ExecuteOp(alu),
                    rcov => rcov.ExecuteRecover(alu),
                    jump => jump.ExecuteJump(alu),
                    recv => recv.ExecuteReceive(alu));
            }

        }

        public class Hypervisor
        {
            public Alu[] Alus { get; set; }
            //public string[] SourceCode { get; set; }

            public void Boot(string input)
            {
                //SourceCode = GetLines(input);
                Alus = new Alu[2]
                {
                    new Alu(this, 0, input, firmwareVersion: 2),
                    new Alu(this, 1, input, firmwareVersion: 2)
                };
                Alus[0].Registers["p"] = 0;
                Alus[1].Registers["p"] = 1;
            }

            public void Execute()
            {
                do
                {
                    foreach (var alu in Alus)
                    {
                        alu.ExecuteProgram();
                    }
                } while (!(Alus.All(a => a.HasRegValue(_FINISHED)
                                         || (a.HasRegValue(_WAITING) && !a.ReceiveQueue.Any()))
                        ));
            }

            public void Send(Alu alu, long value)
            {
                Alus[alu.AluRef == 0 ? 1 : 0].ReceiveQueue.Enqueue(value);
            }
        }
        public class Alu
        {
            private Instruction[] Instructions { get;}
            public Hypervisor Hypervisor { get; set; }
            public Dictionary<string, long> Registers { get; set; }
            public Queue<long> ReceiveQueue { get; set; }
            public long Ptr { get; set; }
            public int AluRef { get;}
            public int FirmwareVersion { get; }
            public int SendCount { get; set; }

            public Alu(string input, int firmwareVersion = 1) : this(null, 0, input, firmwareVersion)
            {
            }

            public Alu(Hypervisor hypervisor, int aluRef, string input, int firmwareVersion = 1)
            {
                Hypervisor = hypervisor;
                AluRef = aluRef;
                FirmwareVersion = firmwareVersion;
                Registers = new Dictionary<string, long>();
                SendCount = 0;
                Ptr = 0;
                var lines = GetLines(input);
                Instructions = new Instruction[lines.Length];
                ReceiveQueue = new Queue<long>();
                foreach (var line in lines)
                {
                    Instructions[Ptr++] = Instruction.GetInstruction(line, firmwareVersion);
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

            public bool HasRegValue(Token.Register reg)
            {
                return Registers.ContainsKey(reg.RegName);
            }

            public void RemoveRegValue(Token.Register reg)
            {
                if (HasRegValue(reg)) Registers.Remove(reg.RegName);
            }

            public void AdjustPtr(long offset)
            {
                Ptr += offset;
            }

            public void ExecuteProgram(bool haltOnReceive = false)
            {
                while (Ptr >= 0 && Ptr < Instructions.Length && (!haltOnReceive || !HasRegValue(_RECEIVE)))
                {
                    if (FirmwareVersion == 2)
                    {
                        if (ReceiveQueue.Any() && !HasRegValue(_RECEIVE))
                        {
                            SetRegValue(_RECEIVE, ReceiveQueue.Dequeue());
                            RemoveRegValue(_WAITING);
                        }
                    }

                    Instructions[Ptr].Execute(this);

                    if (FirmwareVersion == 2)
                    {
                        if (HasRegValue(_SEND))
                        {
                            Hypervisor.Send(this, GetRegValue(_SEND));
                            SendCount++;
                            RemoveRegValue(_SEND);
                        }

                        if (HasRegValue(_WAITING)) return;
                    }
                    Ptr++;
                }
                SetRegValue(_FINISHED,1);
            }
        }

        public override string First(string input)
        {
            var alu = new Alu(input);
            alu.ExecuteProgram(haltOnReceive: true);
            return alu.GetRegValue(_RECEIVE).ToString();
        }

        public override string Second(string input)
        {
            Hypervisor hv = new Hypervisor();
            hv.Boot(input);
            hv.Execute();
            return hv.Alus[1].SendCount.ToString();
        }

    }
}
