
import axios from "axios";

const API_URL = "http://localhost:5050/api/ProductVendor"; // Backend API URL

// ✅ Get all absolute products
export const getAllAbsoluteProducts = async () => {
    return await axios.get(`${API_URL}/GetAll`);
};

// ✅ Get product by ID
export const getProductById = async (productId) => {
    return await axios.get(`${API_URL}/GetById/${productId}`);
};

// ✅ Update absolute product
export const updateAbsoluteProduct = async (productId, productData) => {
    return await axios.put(`${API_URL}/UpdateAbsoluteProduct/${productId}`, productData);
};

// ✅ Get relative product valid units
export const getRelativeProductValidUnits = async (relativeProductID) => {
    return await axios.get(`${API_URL}/GetRelativeProductValidUnits/${relativeProductID}`);
};
