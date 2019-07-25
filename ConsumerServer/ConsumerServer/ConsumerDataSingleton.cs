using System;
using System.Collections.Generic;

public class ConsumerData
{
    private static ConsumerData instance;

    private ConsumerData() { }

    public static ConsumerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConsumerData();
            }
            return instance;
        }
    }

    public List<string> Messages { get; set; }
}