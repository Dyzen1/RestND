using RestND.Data;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestND.Validations
{
    public class InventoryValidator : GeneralValidations
    {
        private readonly ProductService _productService = new();

        // ----------------- Null / existence -----------------

        public bool CheckIfNull(Inventory product, out string err)
        {
            err = string.Empty;
            if (product == null)
            {
                err = "You must choose/create a product.";
                return false;
            }
            return true;
        }

        public bool ExistsById(string productId, IEnumerable<Inventory> existing, out string err)
        {
            err = string.Empty;
            if (existing == null || !existing.Any(p =>
                    !string.IsNullOrWhiteSpace(p.Product_ID) &&
                    p.Product_ID.Equals(productId, StringComparison.OrdinalIgnoreCase)))
            {
                err = "Product does not exist.";
                return false;
            }
            return true;
        }

        // ----------------- Field-level checks (Dish-style) -----------------

        /// <summary>
        /// ID must be non-empty, match format, and (optionally) be unique.
        /// </summary>
        public bool ValidId(
            string id,
            out string err,
            IEnumerable<Inventory>? existing = null,
            bool checkExists = true,
            string? excludeId = null)
        {
            err = string.Empty;

            // Not empty
            if (!IsEmptyField(id, out var emptyErr))
            {
                err = emptyErr; // e.g., "Field cannot be empty"
                return false;
            }

            // Format (same idea as your previous isSerialNumValid)
            // 2–50 non-whitespace chars.
            var pattern = @"^\S{2,50}$";
            if (!Regex.IsMatch(id, pattern))
            {
                err = "Invalid serial number!";
                return false;
            }

            // Uniqueness
            if (checkExists && existing != null)
            {
                var exists = existing.Any(p =>
                    !string.IsNullOrWhiteSpace(p.Product_ID) &&
                    p.Product_ID.Equals(id, StringComparison.OrdinalIgnoreCase) &&
                    (excludeId == null || !p.Product_ID.Equals(excludeId, StringComparison.OrdinalIgnoreCase)));

                if (exists)
                {
                    err = "Product ID already exists.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Name must be non-empty, valid format, and (optionally) unique.
        /// </summary>
        public bool ValidName(
            string name,
            out string err,
            IEnumerable<Inventory>? existing = null,
            bool checkExists = true,
            string? excludeId = null)
        {
            err = string.Empty;

            // Not empty
            if (!IsEmptyField(name, out var emptyErr))
            {
                err = emptyErr;
                return false;
            }

            // Format (reuse your dish name rules)
            if (!isNameValid(name, out var formatErr))
            {
                err = formatErr;
                return false;
            }

            // Uniqueness
            if (checkExists && existing != null)
            {
                var exists = existing.Any(p =>
                    !string.IsNullOrWhiteSpace(p.Product_Name) &&
                    p.Product_Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    // Exclude same product by ID (for updates)
                    (excludeId == null || !string.Equals(p.Product_ID, excludeId, StringComparison.OrdinalIgnoreCase)));

                if (exists)
                {
                    err = "Product name already exists.";
                    return false;
                }
            }

            return true;
        }

        public bool ValidQuantity(string quantityInput, out int parsed, out string err)
        {
            err = string.Empty;
            parsed = 0;

            if (!int.TryParse(quantityInput, out parsed))
            {
                err = "Quantity must be a valid number.";
                return false;
            }

            if (!CheckPosNum(parsed, out var posErr))
            {
                err = posErr;
                return false;
            }

            return true;
        }

        public bool ValidTolerance(string toleranceInput, out double parsed, out string err)
        {
            err = string.Empty;
            parsed = 0;

            if (!double.TryParse(toleranceInput, out parsed))
            {
                err = "Tolerance must be a valid number.";
                return false;
            }

            if (!CheckPosNumDouble(parsed, out var posErr))
            {
                err = posErr;
                return false;
            }

            return true;
        }

        // ----------------- Aggregate checks -----------------

        public bool ValidateForAdd(
            string idInput,
            string nameInput,
            string quantityInput,
            string toleranceInput,
            IEnumerable<Inventory> existingProducts,
            out int parsedQuantity,
            out double parsedTolerance,
            out string err)
        {
            parsedQuantity = 0;
            parsedTolerance = 0;
            err = string.Empty;

            if (!ValidId(idInput, out err, existingProducts, checkExists: true)) return false;
            if (!ValidName(nameInput, out err, existingProducts, checkExists: true)) return false;
            if (!ValidQuantity(quantityInput, out parsedQuantity, out err)) return false;
            if (!ValidTolerance(toleranceInput, out parsedTolerance, out err)) return false;

            return true;
        }

        public bool ValidateForUpdate(
            Inventory product,
            IEnumerable<Inventory> existingProducts,
            out string err)
        {
            err = string.Empty;

            if (!CheckIfNull(product, out err)) return false;

            // Ensure the product exists by ID in current list
            if (!ExistsById(product.Product_ID, existingProducts, out err)) return false;

            // Field-level validations (exclude current ID from uniqueness)
            if (!ValidId(product.Product_ID, out err, existingProducts, checkExists: true, excludeId: product.Product_ID))
                return false;

            if (!ValidName(product.Product_Name, out err, existingProducts, checkExists: true, excludeId: product.Product_ID))
                return false;

            // Positive numbers
            if (!CheckPosNum(product.Quantity_Available, out err)) return false;
            if (!CheckPosNumDouble(product.Tolerance, out err)) return false;

            return true;
        }
    }
}
