using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day23 : Day
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

        public class Instruction : OneOfBase<Instruction.Copy, Instruction.Op, Instruction.Recover, Instruction.JumpPositive, Instruction.Receive, Instruction.JumpNotZero>
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

                    case "sub":
                        instruction =
                            new Instruction.Op()
                            {
                                Register = new Token.Register() { RegName = parts[1] },
                                Value = Token.GetToken(parts[2]),
                                Operation = (a, b) => a - b
                            };
                        break;
                    case "mul":
                        instruction =
                            new Instruction.Op()
                            {
                                DebugKey = "mul",
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
                                    Register = new Token.Register() { RegName = parts[1] }
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
                        instruction = new Instruction.JumpPositive()
                        {
                            Compare = Token.GetToken(parts[1]),
                            Offset = Token.GetToken(parts[2])
                        };
                        break;
                    case "jnz":
                        instruction = new Instruction.JumpNotZero()
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
                public string DebugKey { get; set; }
                public Token.Register Register { get; set; }
                public Token Value { get; set; }
                public Func<long, long, long> Operation { get; set; }

                public void ExecuteOp(Alu alu)
                {
                    if (!string.IsNullOrWhiteSpace(DebugKey)) alu.IncreaseDebugCount(DebugKey);

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

            public class JumpPositive : Instruction
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

            public class JumpNotZero : Instruction
            {
                public Token Compare { get; set; }
                public Token Offset { get; set; }

                public void ExecuteJump(Alu alu)
                {
                    if (Compare.Match(reg => alu.GetRegValue(reg), val => val.Value) != 0)
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
                        alu.SetRegValue(_WAITING, 1);
                    }
                }
            }

            public void Execute(Alu alu)
            {
                this.Switch(copy => copy.ExecuteCopy(alu),
                    op => op.ExecuteOp(alu),
                    rcov => rcov.ExecuteRecover(alu),
                    jgz => jgz.ExecuteJump(alu),
                    recv => recv.ExecuteReceive(alu),
                    jnz => jnz.ExecuteJump(alu));
            }

        }

        public class Alu
        {
            private Instruction[] Instructions { get; }
            //public Hypervisor Hypervisor { get; set; }
            public Dictionary<string, long> Registers { get; set; }
            public Queue<long> ReceiveQueue { get; set; }
            public long Ptr { get; set; }
            public int AluRef { get; }
            public int FirmwareVersion { get; }
            public int SendCount { get; set; }
            public Dictionary<string,int> DebugCount { get; set; }

            public Alu(string input, int firmwareVersion = 1)
            //: this(null, 0, input, firmwareVersion)
            {
                //}

                //public Alu(Hypervisor hypervisor, int aluRef, string input, int firmwareVersion = 1)
                //{
                //Hypervisor = hypervisor;
                //AluRef = aluRef;
                FirmwareVersion = firmwareVersion;
                Registers = new Dictionary<string, long>();
                SendCount = 0;
                DebugCount = new Dictionary<string, int>();
                Ptr = 0;
                ReceiveQueue = new Queue<long>();

                var lines = GetLines(input);
                Instructions = new Instruction[lines.Length];
                foreach (var line in lines)
                {
                    Instructions[Ptr++] = Instruction.GetInstruction(line, firmwareVersion);
                }
                Ptr = 0;

            }

            public void IncreaseDebugCount(string key)
            {
                DebugCount[key] = DebugCount.GetValueOrDefault(key) + 1;
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
                //int counter = 0;
                while (Ptr >= 0 && Ptr < Instructions.Length && (!haltOnReceive || !HasRegValue(_RECEIVE)))
                {
                    ReceiveFromQueue();

                    Instructions[Ptr].Execute(this);

                    SendToQueue();

                    if (AwaitingInput()) return;

                    Ptr++;

                    //counter++;
                    //if ((counter % 10000000) == 0) Console.WriteLine("{0}     {1}", Registers.GetValueOrDefault("a"), Registers.GetValueOrDefault("h"));
                }
                SetRegValue(_FINISHED, 1);
            }

            private void SendToQueue()
            {
                if (FirmwareVersion == 2)
                {
                    if (HasRegValue(_SEND))
                    {
                        //Hypervisor.Send(this, GetRegValue(_SEND));
                        SendCount++;
                        RemoveRegValue(_SEND);
                    }
                }
            }

            private bool AwaitingInput()
            {
                return (FirmwareVersion == 2 && HasRegValue(_WAITING));
            }

            private void ReceiveFromQueue()
            {
                if (FirmwareVersion == 2)
                {
                    if (ReceiveQueue.Any() && !HasRegValue(_RECEIVE))
                    {
                        SetRegValue(_RECEIVE, ReceiveQueue.Dequeue());
                        RemoveRegValue(_WAITING);
                    }
                }
            }
        }

        public override string First(string input)
        {
            var alu = new Alu(input);
            alu.ExecuteProgram(haltOnReceive: true);
            return alu.DebugCount["mul"].ToString();
        }

        public override string SecondTest(string input)
        {
            return "";
        }
        public override string Second(string input)
        {
            int b = 107900;
            
            int h = 0;
            do
            {
                bool incH = false;
                var d = 2;
                do
                {
                    var e = 2;
                    do
                    {
                        if ((d * e) == b)
                        {
                            incH = true;
                            
                        }
                        e++;
                    } while (!incH && e != b); // 100000 ish loops(2->b)
                    d++;
                } while (!incH && d != b); //100000 ish loops(2->b)

                if (incH) h++;
                if (b == 124900) return h.ToString(); //break;// exit //1001? loops
                b += 17;
            } while (true);
        }
    }
}
