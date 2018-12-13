using System;

/// <summary>
/// File: DataProcessing.cs
/// The class responsible for data processing instuctions.
/// </summary>
/// 
namespace armsim.Model.Decoders
{
    //The subclass of Instruction that deals with Data Processing instructions
    class DataProcessing : Instruction
    {
        int opn, opd, op3; //The operand values, stored in the registers
        char opcode, Rn, Rd; //opccode: the operation to run on the data, Rn and Rd are the operand registers that hol the operand values
        bool IBit, SBit; //IBit: immediate vs Register, SBit: raise flags or not
        string strShift = ""; //String represntation of second half of instruction (first half is determined in ToString() )
        bool isMUL = false; //isMUL to know the Multiply instruction
        char Rm; //Rm: register for multiplying, MULop: operand for multiplying

        //Indepentent constructor that initializes all its components itself
        public DataProcessing(int code, ref Memory Regs)
        {
            this.code = code;
            this.Regs = Regs;
            type = (char)ExtractBits(code, 25, 27);
            cond = (char)ExtractBits(code, 28, 31);
        }
        //Dependent constructor that relies on outside sources 
        //to fill in the basic components
        public DataProcessing() { }

        //Decodes according to 'code' and returns the modified DataProcessing class 
        //itself with the local variables populated according to the instruction code
        public override Instruction Decode()
        {
            if (type == 0 && ExtractBits(code, 4, 7) == 9) return DecodeMultiply();

            IBit = TestFlag(code, 25);
            SBit = TestFlag(code, 20);
            Rn = (char)ExtractBits(code, 16, 19);
            Rd = (char)ExtractBits(code, 12, 15);
            CalculateOperands();
            opcode = (char)ExtractBits(code, 21, 24);

            return this;
        }

        //Extract the operands op1, op2, op3 from the command
        void CalculateOperands()
        {
            opn = Regs.ReadWord(GetReg(Rn)); //What is stored in Rn
            if (Rn == 15) opn += 8;

            opd = Regs.ReadWord(GetReg(Rd)); //What is stored in Rd
            op3 = IBit ? CalulateImmediateOperand3() : CalculateRegisterOperand3();
        }

        //Calcultes and returns the immediate shift operand
        int CalulateImmediateOperand3()
        {
            short shift = Convert.ToInt16(ExtractBits(code, 8, 11));
            shift *= 2;
            int immediate = ExtractBits(code, 0, 7);
            immediate = BarrelShifter.ShiftByCode(immediate, shift, (char)3);

            strShift = "#" + immediate;

            return immediate;
        }

        //Calcultes and returns the register shift operand
        int CalculateRegisterOperand3()
        {
            int shiftVal;

            Rm = (char)ExtractBits(code, 0, 3);
            char Sh = (char)ExtractBits(code, 5, 6);
            bool bit4 = TestFlag(code, 4);
            char Rs = '0';

            strShift = "r" + (int)Rm;

            if (!bit4)
                shiftVal = ExtractBits(code, 7, 11);
            else
            {
                Rs = (char)ExtractBits(code, 8, 11);
                shiftVal = Regs.ReadWord(GetReg(Rs));
                if (Rs == 15) shiftVal += 8;
            }
            if (shiftVal != 0)
            {
                strShift += ", " + BarrelShifter.CodeToString(Sh) + (bit4 ? " r" + (int)Rs : " #" + shiftVal);
            }

            int val = Regs.ReadWord(GetReg(Rm));
            if (Rm == 15) val += 8;

            return BarrelShifter.ShiftByCode(val, shiftVal, Sh);
        }

        //Decodes the special case multiply instruction, and returns current class populated with rn, rm, rd
        Instruction DecodeMultiply()
        {
            //extract Rd, Rs, Rm, op1
            SBit = TestFlag(code, 20);
            Rd = (char)ExtractBits(code, 16, 19);
            Rn = (char)ExtractBits(code, 8, 11);
            Rm = (char)ExtractBits(code, 0, 3);
            isMUL = true;

            return this;
        }

        //Return string representation of the instruction
        public override string ToString()
        {
            string opcodeStr = suffix;

            if (isMUL)
            {
                opcodeStr += "mul r" + (int)Rd + ", r" + (int)Rm + ", r" + (int)Rn;
            }
            //I don't think i can avoid a huge switch here, any shorter ways would overcomplicate things.
            //Switch is repeated here because decode is independent of execute. 
            //All the data should be present even if Execute was never called. (And Execute depends on Decode)
            else switch (opcode)
                {
                    case (char)0: //0000 AND Logical AND Rd := Rn AND shifter_operand
                        opcodeStr += "and r" + (int)Rd + ", r" + (int)Rn + ", " + strShift;
                        break;
                    case (char)1: //0001 EOR Logical Exclusive OR Rd:= Rn EOR shifter_operand
                        opcodeStr += "eor r" + (int)Rd + ", r" + (int)Rn + ", " + strShift;
                        break;
                    case (char)2: //0010 SUB Subtract Rd:= Rn - shifter_operand
                        opcodeStr += "sub r" + (int)Rd + ", r" + (int)Rn + ", " + strShift;
                        break;
                    case (char)3: //0011 RSB Reverse Subtract Rd := shifter_operand - Rn
                        opcodeStr += "and r" + (int)Rd + ", r" + (int)Rn + ", " + strShift;
                        break;
                    case (char)4: //0100 ADD Add Rd:= Rn + shifter_operand
                        opcodeStr += "add r" + (int)Rd + ", r" + (int)Rn + ", " + strShift;
                        break;
                    case (char)5: //0101 ADC Add with Carry Rd:= Rn + shifter_operand + Carry Flag
                        break;
                    case (char)6: //0110 SBC Subtract with Carry Rd:= Rn - shifter_operand - NOT(Carry Flag)
                        break;
                    case (char)7: //0111 RSC Reverse Subtract with Carry Rd := shifter_operand - Rn - NOT(Carry Flag)
                        break;
                    case (char)8: //1000 TST Test Update flags after Rn AND shifter_operand
                        break;
                    case (char)9: //1001 TEQ Test Equivalence Update flags after Rn EOR shifter_operand
                        break;
                    case (char)10: //1010 CMP Compare Update flags after Rn -shifter_operand
                        opcodeStr += "cmp r" + (int)Rn + ", " + strShift;
                        break;
                    case (char)11: //1011 CMN Compare Negated Update flags after Rn + shifter_operand
                        break;
                    case (char)12: //1100 ORR Logical(inclusive) OR Rd := Rn OR shifter_operand
                        opcodeStr += "orr r" + (int)Rd + ", r" + (int)Rn + ", " + strShift;
                        break;
                    case (char)13: //1101 MOV Move Rd:= shifter_operand(no first operand)
                        opcodeStr += "mov" + (SBit ? "s " : " ") + "r" + (int)Rd + ", " + strShift;
                        break;
                    case (char)14: //1110 BIC Bit Clear Rd := Rn AND NOT(shifter_operand)
                        opcodeStr += "bic r" + (int)Rd + ", r" + (int)Rn + ", " + strShift;
                        break;
                    case (char)15: //1111 MVN Move Not Rd := NOT shifter_operand (no first operand)
                        opcodeStr += "mvn r" + (int)Rd + ", " + strShift;
                        break;
                }
            return AliasInstructions(opcodeStr);
        }

        //Executes the operation according to the opcode value
        public override void Execute()
        {
            if (isMUL) //for the multiply special case, this is its own execution path.
            {
                ExecMUL();
                return;
            }

            //I don't think i can avoid a huge switch here, any shorter ways would overcomplicate things.
            switch (opcode)
            {
                case (char)0: //0000 AND Logical AND Rd := Rn AND shifter_operand
                    AND();
                    break;
                case (char)1: //0001 EOR Logical Exclusive OR Rd:= Rn EOR shifter_operand
                    EOR();
                    break;
                case (char)2: //0010 SUB Subtract Rd:= Rn - shifter_operand
                    SUB();
                    break;
                case (char)3: //0011 RSB Reverse Subtract Rd := shifter_operand - Rn
                    RSB();
                    break;
                case (char)4: //0100 ADD Add Rd:= Rn + shifter_operand
                    ADD();
                    break;
                case (char)5: //0101 ADC Add with Carry Rd:= Rn + shifter_operand + Carry Flag
                    ADC();
                    break;
                case (char)6: //0110 SBC Subtract with Carry Rd:= Rn - shifter_operand - NOT(Carry Flag)
                    SBC();
                    break;
                case (char)7: //0111 RSC Reverse Subtract with Carry Rd := shifter_operand - Rn - NOT(Carry Flag)
                    RSC();
                    break;
                case (char)8: //1000 TST Test Update flags after Rn AND shifter_operand
                    TST();
                    break;
                case (char)9: //1001 TEQ Test Equivalence Update flags after Rn EOR shifter_operand
                    TEQ();
                    break;
                case (char)10: //1010 CMP Compare Update flags after Rn -shifter_operand
                    CMP();
                    break;
                case (char)11: //1011 CMN Compare Negated Update flags after Rn + shifter_operand
                    CMN();
                    break;
                case (char)12: //1100 ORR Logical(inclusive) OR Rd := Rn OR shifter_operand
                    ORR();
                    break;
                case (char)13: //1101 MOV Move Rd:= shifter_operand(no first operand)
                    MOV();
                    break;
                case (char)14: //1110 BIC Bit Clear Rd := Rn AND NOT(shifter_operand)
                    BIC();
                    break;
                case (char)15: //1111 MVN Move Not Rd := NOT shifter_operand (no first operand)
                    MVN();
                    break;
            }
        }

        //performs the special case for the multiplication instruction,
        //multiplies the values in Rm and Rn and stores their results in Rd
        void ExecMUL()
        {
            Regs.WriteWord(GetReg(Rd), (Regs.ReadWord(GetReg(Rn)) * Regs.ReadWord(GetReg(Rm))));
        }

        //All these operations are unique in their own ways and I cannot simplify them into one function, 
        //they all need their own functions. They are also highly dependent on the same instance variables, therefore 
        //making a utilities class would mean many unnecessary arguments.
        //All the operations:
        //move value of op3 into Rd
        void MOV()
        {
            Regs.WriteWord(GetReg(Rd), op3);
            if (SBit && Rd == 15)
                SwitchBackMode();
        }
        // Rn + op3 and store in Rd
        void ADD()
        {
            Regs.WriteWord(GetReg(Rd), op3 + opn);
        }
        // Rn AND op3 and store in Rd
        void AND()
        {
            Regs.WriteWord(GetReg(Rd), op3 & opn);
        }
        // Rn EOR op3 and store in Rd
        void EOR()
        {
            Regs.WriteWord(GetReg(Rd), opn ^ op3);
        }
        // Rn - op3 and store in Rd
        void SUB()
        {
            Regs.WriteWord(GetReg(Rd), opn - op3);
        }
        // op3 - Rn and store in Rd
        void RSB()
        {
            Regs.WriteWord(GetReg(Rd), op3 - opn);
        }
        // Rn + op3 + carry_flag and store in Rd
        void ADC()
        {
            //Regs.WriteWord(GetReg(Rd), opn ^ op3);
            //rd = rn + op3 + Carry_flag
        }
        // Rn - op3 - NOT(carry_Flag) and store in Rd
        void SBC()
        {
            //rd = rn - op3 - ~carry_flag
        }
        // op3 - rn - NOT(carry_Flag) and store in Rd
        void RSC()
        {
            //rd = op3 - rn - ~carry_flag
        }
        // Test update flags after rn AND op3
        void TST()
        {
            //test rn AND op3
        }
        // test equivalnce update flags after rn EOR op3
        void TEQ()
        {
            //test rn EOR op3
        }
        // Test flags after rn - op3
        void CMP()
        {
            int res = opn - op3;
            if (res < 0) //n flag
                SetCPSRFlag(31, true);
            else SetCPSRFlag(31, false);

            if (res == 0) //z flag
                SetCPSRFlag(30, true);
            else SetCPSRFlag(30, false);

            //c flag

            if ((uint)op3 > (uint)opn)
                SetCPSRFlag(29, false);
            else
                SetCPSRFlag(29, true);

            //v flag
            if (opn >= 0 && op3 < 0 && res < 0)
                SetCPSRFlag(28, true);
            else if (opn < 0 && op3 >= 0 && res >= 0)
                SetCPSRFlag(28, true);
            else SetCPSRFlag(28, false);
        }
        // test negated update flag
        void CMN()
        {
            //test rn + op3
        }
        // rn logical OR op3 store in rd
        void ORR()
        {
            //rd = rn || op3
            Regs.WriteWord(GetReg(Rd), opn | op3);
        }
        //bit clear rd = rn & ~op3
        void BIC()
        {
            Regs.WriteWord(GetReg(Rd), opn & ~op3);
        }
        // move NOT op3 into rd
        void MVN()
        {
            Regs.WriteWord(GetReg(Rd), ~op3);
        }
    }
}
