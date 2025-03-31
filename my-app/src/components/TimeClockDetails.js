import React, { useEffect, useState } from 'react';
import { getTimeEntriesByUserId, punchIn, punchOut } from '../services/timeClockService';

const TimeClockDetails = ({ userId }) => {
    const [timeEntries, setTimeEntries] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isPunchedIn, setIsPunchedIn] = useState(false);

    useEffect(() => {
        if (userId) {
            fetchTimeEntries(userId);
        }
    }, [userId]);

    const fetchTimeEntries = async (userId) => {
        try {
            setLoading(true);
            setError(null);
            const entries = await getTimeEntriesByUserId(userId);
            setTimeEntries(entries);
            setIsPunchedIn(entries.some(entry => !entry.ServerTimeOut)); // If there's an entry without timeout, user is punched in
        } catch (error) {
            setError(error.message || 'Error fetching time entries.');
            console.error('Error fetching time entries:', error);
        } finally {
            setLoading(false);
        }
    };

    const handlePunchIn = async () => {
        try {
            await punchIn(userId);
            await fetchTimeEntries(userId);
        } catch (error) {
            setError(error.message || 'Error punching in.');
            console.error('Error punching in:', error);
        }
    };

    const handlePunchOut = async () => {
        try {
            await punchOut(userId);
            await fetchTimeEntries(userId);
        } catch (error) {
            setError(error.message || 'Error punching out.');
            console.error('Error punching out:', error);
        }
    };

    return (
        <div>
            <h3>Time Clock Details for User {userId}</h3>
            {error && <div className="alert alert-danger">{error}</div>}
            <div>
                <button onClick={handlePunchIn} disabled={isPunchedIn}>Punch In</button>
                <button onClick={handlePunchOut} disabled={!isPunchedIn}>Punch Out</button>
            </div>
            <h4>Time Entries:</h4>
            {loading ? <div>Loading...</div> : timeEntries.length === 0 ? (
                <div>No time entries found.</div>
            ) : (
                <table className="table">
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Time In</th>
                            <th>Time Out</th>
                            <th>Total Hours</th>
                        </tr>
                    </thead>
                    <tbody>
                        {timeEntries.map((entry) => (
                            <tr key={entry.TMID}>
                                <td>{new Date(entry.ServerTimeIn).toLocaleDateString()}</td>
                                <td>{new Date(entry.ServerTimeIn).toLocaleTimeString()}</td>
                                <td>{entry.ServerTimeOut ? new Date(entry.ServerTimeOut).toLocaleTimeString() : 'Pending'}</td>
                                <td>{entry.TotalHours ?? 'N/A'}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default TimeClockDetails;
