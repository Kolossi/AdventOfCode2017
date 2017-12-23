using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{

    // see 2017 day 18 (and 2016 day 12)
    class Day23 : Day
    {
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

        public class Instruction : OneOfBase<Instruction.Copy, Instruction.Op, Instruction.JumpPositive, Instruction.JumpNotZero>
        {
            public static Instruction GetInstruction(string input)
            {
                var parts = GetParts(input);
                Instruction instruction;
                switch (parts[0])
                {
                    case "set":
                        instruction = new Instruction.Copy()
                        {
                            Source = Token.GetToken(parts[2]),
                            Destination = new Token.Register() { RegName = parts[1] }
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

            public void Execute(Alu alu)
            {
                this.Switch(copy => copy.ExecuteCopy(alu),
                    op => op.ExecuteOp(alu),
                    jgz => jgz.ExecuteJump(alu),
                    jnz => jnz.ExecuteJump(alu));
            }

        }

        public class Alu
        {
            private Instruction[] Instructions { get; }
            public Dictionary<string, long> Registers { get; set; }
            public Queue<long> ReceiveQueue { get; set; }
            public long Ptr { get; set; }
            public int AluRef { get; }
            public int SendCount { get; set; }
            public Dictionary<string,int> DebugCount { get; set; }

            public Alu(string input)
            {
                Registers = new Dictionary<string, long>();
                SendCount = 0;
                DebugCount = new Dictionary<string, int>();
                Ptr = 0;
                ReceiveQueue = new Queue<long>();

                var lines = GetLines(input);
                Instructions = new Instruction[lines.Length];
                foreach (var line in lines)
                {
                    Instructions[Ptr++] = Instruction.GetInstruction(line);
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

            public void ExecuteProgram()
            {
                while (Ptr >= 0 && Ptr < Instructions.Length)
                {
                    Instructions[Ptr].Execute(this);
                    Ptr++;
                }
            }
        }

        public override string First(string input)
        {
            var alu = new Alu(input);
            alu.ExecuteProgram();
            return alu.DebugCount["mul"].ToString();
        }

        public override string SecondTest(string input)
        {
            return "The second test file has the input decompiled";
        }

        public override string Second(string input)
        {
            //return SecondInCSharp();
            var alu = new Alu(PatchProgram(input));
            alu.Registers["a"] = 1;
            alu.ExecuteProgram();
            return alu.Registers["h"].ToString();
        }
        
        public string PatchProgram(string input)
        {
            input = input
                .Replace("set g d\r\nmul g e\r\nsub g b",
                         "set g b\r\nmod g e\r\njnz 0 0")
                .Replace("set f 0", "jnz 1 10")
                .Replace("jnz g -13", "jnz 0 0");
            return input;
        }
        public string SecondInCSharp()
        {
            int b = 107900;
            
            int h = 0;
            do
            {
                bool incH = false;
                //var d = 2;
                //do
                //{
                    var e = 2;
                    do
                    {
                        if (b % e == 0)
                        //if ((d * e) == b)
                        {
                            incH = true;
                            
                        }
                        e++;
                    } while (!incH && e != b); // 100000 ish loops(2->b)
                //d++;
                //} while (!incH && d != b); //100000 ish loops(2->b)

            if (incH) h++;
                if (b == 124900) return h.ToString(); //break;// exit //1001? loops
                b += 17;
            } while (true);
        }

        //  1 set b 79
        //  2 set c b
        //  3 jnz a 2
        //  4 jnz 1 5
        //  5 mul b 100
        //  6 sub b -100000
        //  7 set c b
        //  8 sub c -17000
        //  9 set f 1
        // 10 set d 2
        // 11 set e 2
        // 12 set g b # was set g d // instead of trying to find d,e where d*e=b
        // 13 mod g e # was mul g e // just find any e which is a factor of b (b%e==0)
        // 14 jnz 0 0 # was sub g b // we then don't need the outer (d) loop
        // 15 jnz g 2
        // 16 jnz 1 10 # was set f 0 // this way we exit the loop when a match is found
        // 17 sub e -1
        // 18 set g e
        // 19 sub g b
        // 20 jnz g -8
        // 21 sub d -1
        // 22 set g d
        // 23 sub g b
        // 24 jnz 0 0 # was jnz g -13
        // 25 jnz f 2
        // 26 sub h -1
        // 27 set g b
        // 28 sub g c
        // 29 jnz g 2
        // 30 jnz 1 3
        // 31 sub b -17
        // 32 jnz 1 -23


    }
}
