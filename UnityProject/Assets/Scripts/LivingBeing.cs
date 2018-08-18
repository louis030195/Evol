﻿
using System.ComponentModel;

public abstract class LivingBeing {

    protected LivingBeing(int life, int age, int lifeExpectancy, int satiety, int tiredness)
    {
        Life = life;
        Age = age;
        LifeExpectancy = lifeExpectancy;
        Satiety = satiety;
        Tiredness = tiredness;
    }

    public float Satiety { get; set; }

    public float Life { get; set; }

    public float Age { get; set; }

    public float LifeExpectancy { get; set; }

    public float Tiredness { get; set; }

    public override string ToString()
    {
        string attributesString = "";
        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
        {
            string name = descriptor.Name;
            object value = descriptor.GetValue(this);
            attributesString += $"{name}={value}\n";
        }
        return attributesString;
    }
}
