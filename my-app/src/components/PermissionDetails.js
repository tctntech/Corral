import React, { useEffect, useState } from 'react';
import { getPermissionById } from '../services/PermissionService';

const PermissionDetails = ({ permissionId }) => {
    const [permission, setPermission] = useState(null);

    useEffect(() => {
        if (permissionId) {
            fetchPermissionDetails(permissionId);
        }
    }, [permissionId]);

    const fetchPermissionDetails = async (id) => {
        try {
            const data = await getPermissionById(id);
            console.log('Fetched Permission Details:', data);
            if (data.success) {
                setPermission(data.data);
            } else {
                console.error(data.message || 'Error fetching permission details');
            }
        } catch (error) {
            console.error('Error fetching permission details:', error);
        }
    };

    if (!permission) {
        return <div>Select a permission to view details</div>;
    }

    return (
        <div>
            <h3>Permission Details</h3>
            <table>
                <tbody>
                    <tr>
                        <td>Permission Name:</td>
                        <td>{permission.PermissionName}</td>
                    </tr>
                    <tr>
                        <td>Description:</td>
                        <td>{permission.PermissionDescription}</td>
                    </tr>
                    <tr>
                        <td>Module:</td>
                        <td>{permission.FoundInModule}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
};

export default PermissionDetails;
