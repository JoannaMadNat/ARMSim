/// <summary>
/// File: NoOp.cs
/// This class does nothing, and is only here so that the code value 0 does not go unhandled
/// </summary>
namespace armsim.Model.Decoders
{
    //Loosely bassed on the 'No operation' instruction, 
    //but actually does nothing in the context of this simulator
    //it is only here to handle when code = 0
    class NoOp : Instruction
    {
        //Does not have a string representation
        public override string ToString()
        {
            return "";
        }

        //Nothing to decode. code is 0
        public override Instruction Decode()
        {
            return this;
        }

        //Does nothing
        public override void Execute()
        {
            return;
        }
    }
}
