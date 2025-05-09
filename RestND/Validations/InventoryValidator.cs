using System;
using System.Collections.Generic;
using System.Linq;
using RestND.MVVM.Model;

namespace RestND.Validations
{
    public static class InventoryValidator
    {
        // Method to validate the fields of the Inventory
        public static Dictionary<string, List<string>> ValidateFields(Inventory inventory, List<Inventory> existingInventories)
        {
            var errors = new Dictionary<string, List<string>>();

            // Validate Product Name
            if (string.IsNullOrWhiteSpace(inventory.Product_Name))
            {
                AddError(errors, nameof(inventory.Product_Name), "Product name is required.");
            }
            else if (inventory.Product_Name.Length < 3)
            {
                AddError(errors, nameof(inventory.Product_Name), "Product name must be at least 3 characters long.");
            }
            else if (existingInventories.Any(i => i.Product_Name.Equals(inventory.Product_Name, StringComparison.OrdinalIgnoreCase)))
            {
                AddError(errors, nameof(inventory.Product_Name), "Product name already exists in the inventory.");
            }

            // Validate Quantity Available
            if (inventory.Quantity_Available < 0)
            {
                AddError(errors, nameof(inventory.Quantity_Available), "Quantity available cannot be negative.");
            }

            return errors;
        }

        // Helper method to add errors to the dictionary
        private static void AddError(Dictionary<string, List<string>> dict, string key, string message)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new List<string>();
            dict[key].Add(message);
        }
    }
}
