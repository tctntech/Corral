
import React, { useState } from "react";
import UserList from "../components/UserList";
import UserDetails from "../components/UserDetails";

const UserManagement = () => {
    const [selectedUserId, setSelectedUserId] = useState(null);

    const handleSelectUser = (userId) => {
        setSelectedUserId(userId);
    };

    return (
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
            <UserList onSelectUser={handleSelectUser} />
            <UserDetails userId={selectedUserId} />
        </div>
    );
};

export default UserManagement;
