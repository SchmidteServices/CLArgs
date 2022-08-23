﻿using MSPro.CLArgs;



namespace SampleCommands.Math.Sum;

[Command("MATH.SUM", "Add any number of integer values.")]
public class SumCommand : CommandBase2<SumContext>
{
    public SumCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override void Execute()
    {
        Console.WriteLine($"{this.Context.Values.Count} values specified:");

        int total = 0;
        foreach (string sValue in this.Context.Values)
        {
            if( int.TryParse(sValue, out int value)) total += value;
        }
        
        Console.WriteLine($"{string.Join("+", this.Context.Values)} = {total}");
    }
}