import React from "react";

const AbsoluteProducts = ({ products }) => {
    return (
        <div>
            <h2>Absolute Products</h2>
            <table border="1">
                <thead>
                    <tr>
                        <th>Product ID</th>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Inventory Units</th>
                    </tr>
                </thead>
                <tbody>
                    {products.map((product) => (
                        <tr key={product.productID}>
                            <td>{product.productID}</td>
                            <td>{product.productName}</td>
                            <td>{product.productDescription}</td>
                            <td>{product.inventoryUnitCtTxt}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default AbsoluteProducts;
