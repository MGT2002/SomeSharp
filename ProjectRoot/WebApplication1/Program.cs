using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => {
    return "Hello World!";
});

app.MapGet("/IdCheck/{id}", (Super id) => {
    return id;
});

app.MapGet("/square/{num}", (int num) => {
    return num * num;
});

app.Run();

class Super : IParsable<Super>, ISuper
{
    [RegularExpression("^ID\\d{1,3}$")]
    public string? Code { get; set; }

    static void DefaultState<T>(T super) where T : ISuper
    {
        T.DefaultState(super);
    }

    static void DefaultState(ISuper super)
    {
        super.Code = "ID001";
    }

    public static Super Parse(
        string s,
        IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result) ? result : throw new InvalidCastException();
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Super result)
    {
        provider.Log("Provider->");

        result = new Super() { Code = s ?? "" };
        //if (Validator.TryValidateProperty(result.Code, new(result) {MemberName=nameof(Code) }, null))
        if (Validator.TryValidateObject(result, new(result), null, true))
        {             
            return true;        
        }

        "Error Binding".Log();
        Create().Code.Log("NewVersion->");
        CreateOld().Code.Log("OldVersion->");
        return false;
    }

    private Super() {}

    public static Super Create()
    { 
        var sup = new Super();
        DefaultState<Super>(sup);
        return sup;
    }

    public static ISuper CreateOld()
    {
        ISuper sup = new Super();
        DefaultState<ISuper>(sup);
        return sup;
    }
}

interface ISuper
{  
    string? Code { get; set; }
    virtual static void DefaultState(ISuper super)
    {
        super.Code = "ID0";
    }
}