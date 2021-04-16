using System;

[Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue;
    public FloatVariable Variable;

    public FloatReference()
    { }

    public FloatReference(float value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public FloatReference(FloatVariable variable)
    {
        UseConstant = false;
        Variable = variable;
    }

    public float Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }

    public static implicit operator float(FloatReference reference)
    {
        if (reference != null)
            return reference.Value;
        else
            return 0;
    }
}