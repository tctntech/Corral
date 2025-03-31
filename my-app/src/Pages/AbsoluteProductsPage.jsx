import React, { useEffect, useState } from "react";
import { getAllAbsoluteProducts } from "../services/productService";
import AbsoluteProducts from "../components/products/AbsoluteProducts";

const AbsoluteProductsPage = () => {
    const [products, setProducts] = useState([]);

    useEffect(() => {
        fetchProducts();
    }, []);

    const fetchProducts = async () => {
        try {
            const response = await getAllAbsoluteProducts();
            setProducts(response.data);
        } catch (error) {
            console.error("Error fetching products:", error);
        }
    };

    return (
        <div className="p-4">
            <h2 className="text-xl font-bold mb-4">Absolute Products</h2>
            <AbsoluteProducts products={products} />
        </div>
    );
};

export default AbsoluteProductsPage;
