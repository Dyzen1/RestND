using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RestND.MVVM.Model.Dishes
{
    public class AllergenNotes
    {
        public  List<string> Allergens = new List<string>
        {
            { "Contains Gluten/Wheat." },
            { "Contains Peanuts." },
            { "Contains Tree Nuts." },
            { "Contains Milk Or Dairy Ingredients." },
            { "Contains Eggs Or Egg-Derived Ingredients." },
            { "Contains Soy Or Soy-Derived Ingredients." },
            { "Contains Fish." },
            { "Contains Shellfish." },
            { "Contains Sesame." },
            { "Contains Mustard." },
            { "Contains Celery." },
            { "Contains Sulfites." }
        };
    }
}

