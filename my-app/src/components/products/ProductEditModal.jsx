import React, { useState } from "react";
import { updateAbsoluteProduct } from "../../services/productService";

const ProductEditModal = ({ product, onClose }) => {
    const [formData, setFormData] = useState({ ...product });

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await updateAbsoluteProduct(product.ProductID, formData);
            alert("Product updated successfully!");
            onClose();
        } catch (error) {
            console.error("Error updating product:", error);
        }
    };

    return (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-900 bg-opacity-50">
            <div className="bg-white p-6 rounded-lg shadow-lg w-1/3">
                <h2 className="text-xl font-bold mb-4">Edit Product</h2>
                <form onSubmit={handleSubmit}>
                    <div className="mb-4">
                        <label className="block text-sm font-medium">Name:</label>
                        <input
                            type="text"
                            name="ProductName"
                            value={formData.ProductName}
                            onChange={handleChange}
                            className="border p-2 w-full rounded"
                        />
                    </div>
                    <div className="mb-4">
                        <label className="block text-sm font-medium">Description:</label>
                        <input
                            type="text"
                            name="ProductDescription"
                            value={formData.ProductDescription}
                            onChange={handleChange}
                            className="border p-2 w-full rounded"
                        />
                    </div>
                    <div className="flex justify-end gap-2">
                        <button type="button" className="bg-gray-400 px-3 py-1 rounded" onClick={onClose}>
                            Cancel
                        </button>
                        <button type="submit" className="bg-green-500 text-white px-3 py-1 rounded">
                            Save
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default ProductEditModal;
