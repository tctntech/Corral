import React, { useState } from 'react';
import PermissionList from '../components/PermissionList';
import PermissionDetails from '../components/PermissionDetails';

const PermissionManagement = ({ managerID }) => {
    const [selectedPermission, setSelectedPermission] = useState(null);

    const handleSelectPermission = (permissionId) => {
        setSelectedPermission(permissionId);
    };

    return (
        <div>
            <h2>Permission Management</h2>
            <PermissionList managerID={managerID} onSelectPermission={handleSelectPermission} />
            {selectedPermission && <PermissionDetails permissionId={selectedPermission} />}
        </div>
    );
};

export default PermissionManagement;
