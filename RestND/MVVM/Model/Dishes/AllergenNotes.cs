using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RestND.MVVM.Model.Dishes
{
    public enum AllergenNotes
    {
        [Description("⚠️ PEANUT Allergy – Contains peanuts.")]
        Peanut,
        [Description("⚠️ TREE NUT Allergy – Contains tree nuts.")]
        TreeNut,
        [Description("⚠️ DAIRY Allergy – Contains milk or dairy products.")]
        Dairy,
        [Description("⚠️ EGG Allergy – Contains egg or egg-derived ingredients.")]
        Egg,
        [Description("⚠️ GLUTEN/WHEAT Allergy – Contains wheat (gluten).")]
        Gluten,
        [Description("⚠️ SOY Allergy – Contains soy or soy-derived ingredients.")]
        Soy,
        [Description("⚠️ FISH Allergy – Contains fish.")]
        Fish,
        [Description("⚠️ SHELLFISH Allergy – Contains shellfish.")]
        Shellfish,
        [Description("⚠️ SESAME Allergy – Contains sesame.")]
        Sesame,
        [Description("⚠️ MUSTARD Allergy – Contains mustard.")]
        Mustard,
        [Description("⚠️ CELERY Allergy – Contains celery.")]
        Celery,
        [Description("⚠️ SULFITE Sensitivity – Contains sulfites.")]
        Sulfite
        
}
}

