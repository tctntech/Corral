import axios from 'axios';

const API_URL = 'http://localhost:5050/users/modules'; // Modules API endpoint

// Fetch modules by Manager ID
export const getModulesByManagerId = async (managerID) => {
    try {
        const response = await axios.get(`${API_URL}/${managerID}`);
        return response.data;
    } catch (error) {
        console.error('Error fetching modules: ', error);
        return { success: false, message: 'Failed to fetch modules', data: [] };
    }
};

// Fetch module details by module ID
export const getModuleById = async (moduleId) => {
    try {
        const response = await axios.get(`${API_URL}/module/${moduleId}`); // Update endpoint if needed
        return response.data;
    } catch (error) {
        console.error('Error fetching module details', error);
        return { success: false, message: 'Failed to fetch module details', data: null };
    }
};
