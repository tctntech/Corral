import axios from 'axios';

const API_URL = 'http://localhost:5050/users/permissions'; // Permissions API endpoint

// Fetch permissions by Manager ID
export const getPermissionsByManagerId = async (managerID) => {
    try {
        const response = await axios.get(`${API_URL}/${managerID}`);
        return response.data;
    } catch (error) {
        console.error('Error fetching permissions: ', error);
        return { success: false, message: 'Failed to fetch permissions', data: [] };
    }
};

// Fetch permission details by permission ID
export const getPermissionById = async (permissionId) => {
    try {
        const response = await axios.get(`${API_URL}/permission/${permissionId}`); // Update endpoint if needed
        return response.data;
    } catch (error) {
        console.error('Error fetching permission details', error);
        return { success: false, message: 'Failed to fetch permission details', data: null };
    }
};
