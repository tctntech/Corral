import axios from 'axios';

const API_BASE_URL = 'http://localhost:5050/timeclock/api';

/**
 * Fetch time entries for a specific user.
 */
export const getTimeEntriesByUserId = async (managerId) => {
    try {
        const response = await axios.get(`${API_BASE_URL}/entries`, { params: { employeeId: managerId } });
        return response.data.Data || [];
    } catch (error) {
        console.error('Error fetching time entries:', error);
        throw new Error(error.response?.data?.message || 'Failed to fetch time entries.');
    }
};

/**
 * Punch in for an employee.
 */
export const punchIn = async (managerId, storeId, rate) => {
    try {
        const response = await axios.post(`${API_BASE_URL}/clockin`, {
            ManagerID: managerId,
            StoreID: storeId,
            Rate: rate,
            TimeIn: new Date().toISOString()
        });
        return response.data;
    } catch (error) {
        console.error('Error punching in:', error);
        throw new Error(error.response?.data?.message || 'Failed to punch in.');
    }
};

/**
 * Punch out for an employee.
 */
export const punchOut = async (managerId) => {
    try {
        const response = await axios.post(`${API_BASE_URL}/clockout`, { ManagerID: managerId });
        return response.data;
    } catch (error) {
        console.error('Error punching out:', error);
        throw new Error(error.response?.data?.message || 'Failed to punch out.');
    }
};

/**
 * Fetch all time clock entries.
 */
export const getAllTimeClockEntries = async () => {
    try {
        const response = await axios.get(`${API_BASE_URL}/entries`);
        return response.data.Data || [];
    } catch (error) {
        console.error('Error fetching time clock entries:', error);
        throw new Error(error.response?.data?.message || 'Failed to fetch time clock entries.');
    }
};
