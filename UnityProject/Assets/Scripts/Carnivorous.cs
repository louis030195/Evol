using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol
{
    public class Carnivorous : LivingBeing
    {

        public Carnivorous(int life, int age, int lifeExpectancy, int satiety, int tiredness, int speed)
            : base(life, age, lifeExpectancy, satiety, tiredness, speed)
        {

        }
    }
}
