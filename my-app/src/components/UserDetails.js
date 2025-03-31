import React, { useEffect, useState } from "react";
import { getUserById } from "../services/UserService";

const UserDetails = ({ userId }) => {
    const [user, setUser] = useState(null);

    useEffect(() => {
        if (userId) {
            fetchUserDetails(userId);
        }
    }, [userId]);

    const fetchUserDetails = async (id) => {
        try {
            const data = await getUserById(id);
            if (data) {
                console.log("Fetched User Details:", data); // Debugging
                setUser(data);
            } else {
                console.error("No user data found");
            }
        } catch (error) {
            console.error("Error fetching user details:", error);
        }
    };

    if (!user) {
        return <div>Select a user to view details</div>;
    }

    return (
        <div className="user-details">
            <h3>User Details</h3>
            <table>
                <tbody>
                    <tr>
                        <td>Username:</td>
                        <td>{user.username}</td>
                    </tr>
                    <tr>
                        <td>Firstname:</td>
                        <td>{user.firstName}</td>
                    </tr>
                    <tr>
                        <td>Full Name:</td>
                        <td>{user.fullName}</td>
                    </tr>
                    <tr>
                        <td>AllowMultiStore :</td>
                        <td>{user.allowmultistore }</td>
                    </tr>
                    <tr>
                        <td>Last Name:</td>
                        <td>{user.lastname}</td>
                    </tr>
                    <tr>
                        <td>Active:</td>
                        <td>{user.active ? "Active" : "Inactive"}</td>
                    </tr>
                    <tr>
                        <td>Account Active Date:</td>
                        <td>{user.accountActiveDate ? user.accountActiveDate : "N/A"}</td>
                    </tr>
                    <tr>
                        <td>Global Staff Link Staff ID:</td>
                        <td>{user.globalStaffLinkStaffID}</td>
                    </tr>
                    <tr>
                        <td>Allow Admin Screen:</td>
                        <td>{user.allowAdminScreen ? "Yes" : "No"}</td>
                    </tr>
                   
                    
                    <tr>
                        <td>Allow Intranet Access:</td>
                        <td>{user.allowIntranetAccess ? "Yes" : "No"}</td>
                    </tr>
                    <tr>
                        <td>Time Clock ID:</td>
                        <td>{user.timeClockID}</td>
                    </tr>
                    <tr>
                        <td>Time Clock Passcode:</td>
                        <td>{user.timeClockPassCode}</td>
                    </tr>
                    <tr>
                        <td>Finger Enrollment Date:</td>
                        <td>{user.fingerEnrollmentDate ? user.fingerEnrollmentDate : "N/A"}</td>
                    </tr>
                    <tr>
                        <td>Allow Type Login:</td>
                        <td>{user.allowTypeLogin ? "Yes" : "No"}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
};

export default UserDetails;
