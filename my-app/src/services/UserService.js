import axios from 'axios';

const API_URL = 'http://localhost:5050/api/users';
const MODULES_API_URL = 'http://localhost:5050/users/modules'; // For the modules endpoint

// Fetch all users
export const getAllUsers = async () => {
    try {
        const response = await axios.get(API_URL);
        return response.data;
    } catch (error) {
        console.error('Error fetching users: ', error);
        return [];
    }
}

// Fetch user details by ID
export const getUserById = async (userID) => {
    try {
        const response = await axios.get(`${API_URL}/${userID}`);
        return response.data;
    } catch (error) {
        console.error("Error fetching user details", error);
        return null;
    }
};

// Fetch modules by Manager ID
export const getModulesByManagerId = async (managerID) => {
    try {
        const response = await axios.get(`${MODULES_API_URL}/${managerID}`);
        return response.data;
    } catch (error) {
        console.error('Error fetching modules: ', error);
        return { success: false, message: 'Failed to fetch modules', data: [] };
    }
};
