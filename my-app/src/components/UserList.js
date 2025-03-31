import React, { useEffect, useState } from "react";
import { getAllUsers } from "../services/UserService";

const UserList = ({ onSelectUser }) => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);  // Added loading state

    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = async () => {
        try {
            const data = await getAllUsers();
            console.log("Fetched users:", data);
            setUsers(data);
        } catch (error) {
            console.error("Error fetching users:", error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return <div>Loading users...</div>;
    }

    return (
        <div>
            <h3>User List</h3>
            <table>
                <thead>
                    <tr>
                        <th>Username</th>
                        <th>Full Name</th>
                        <th>Role</th>
                        <th>Last Login</th>
                    </tr>
                </thead>
                <tbody>
                    {users.length > 0 ? (
                        users.map((user) => (
                            <tr key={user.managerID} onClick={() => onSelectUser(user.managerID)}>
                                <td>{user.username}</td>
                                <td>{user.fullName}</td>
                                <td>{user.active ? "Active" : "Inactive"}</td>
                                <td>{user.accountActiveDate ? user.accountActiveDate : "N/A"}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="4">No users available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default UserList;
