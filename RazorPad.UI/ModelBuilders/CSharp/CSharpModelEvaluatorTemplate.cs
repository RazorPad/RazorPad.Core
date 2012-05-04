using System;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSharpModelEvaluator
{
    public dynamic Model = new ExpandoObject();

    public dynamic GetModel()
    {
        Model.Name = "RazorPad";
        return Model;
    }
}
