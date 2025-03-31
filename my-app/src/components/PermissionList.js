import React, { useEffect, useState } from 'react';
import { getPermissionsByManagerId } from '../services/PermissionService';

const PermissionList = ({ managerID, onSelectPermission }) => {
    const [permissions, setPermissions] = useState([]);

    useEffect(() => {
        if (managerID) {
            fetchPermissions(managerID);
        }
    }, [managerID]);

    const fetchPermissions = async (id) => {
        try {
            const data = await getPermissionsByManagerId(id);
            console.log('Fetched permissions:', data);
            if (data.success) {
                setPermissions(data.data); // Assuming 'data' is inside a 'data' object
            } else {
                console.error(data.message || 'Error fetching permissions');
            }
        } catch (error) {
            console.error('Error fetching permissions:', error);
        }
    };

    return (
        <div>
            <h3>Permission List</h3>
            <table>
                <thead>
                    <tr>
                        <th>Permission Name</th>
                        <th>Description</th>
                        <th>Module</th>
                    </tr>
                </thead>
                <tbody>
                    {permissions.length > 0 ? (
                        permissions.map((permission) => (
                            <tr key={permission.PermissionID} onClick={() => onSelectPermission(permission.PermissionID)}>
                                <td>{permission.PermissionName}</td>
                                <td>{permission.PermissionDescription}</td>
                                <td>{permission.FoundInModule}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="3">No permissions available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default PermissionList;
