import React, { useEffect, useState } from "react";
import { getAllTimeClockEntries } from "../services/timeClockService";

const TimeClockList = ({ onSelectTimeClock }) => {
    const [timeClockEntries, setTimeClockEntries] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchTimeClockEntries();
    }, []);

    const fetchTimeClockEntries = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await getAllTimeClockEntries();
            setTimeClockEntries(data || []);
        } catch (error) {
            setError(error.message || "Error fetching timeclock entries.");
            console.error("Error fetching timeclock entries:", error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mt-3">
            <h3>Time Clock Entries</h3>
            {loading ? (
                <div className="alert alert-info">Loading...</div>
            ) : error ? (
                <div className="alert alert-danger">{error}</div>
            ) : timeClockEntries.length === 0 ? (
                <div className="alert alert-warning">No timeclock entries available.</div>
            ) : (
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Employee ID</th>
                            <th>Clock In</th>
                            <th>Clock Out</th>
                            <th>Hours Worked</th>
                            <th>Approval</th>
                        </tr>
                    </thead>
                    <tbody>
                        {timeClockEntries.map((entry) => (
                            <tr key={entry.TMID} onClick={() => onSelectTimeClock(entry.ManagerID)}>
                                <td>{entry.ManagerID || "N/A"}</td>
                                <td>{entry.ServerTimeIn ? new Date(entry.ServerTimeIn).toLocaleString() : "N/A"}</td>
                                <td>{entry.ServerTimeOut ? new Date(entry.ServerTimeOut).toLocaleString() : "Pending"}</td>
                                <td>{entry.TotalHours !== null ? entry.TotalHours : "N/A"}</td>
                                <td className={entry.isApproved ? "text-success" : "text-warning"}>
                                    {entry.isApproved ? "Approved" : "Pending"}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default TimeClockList;
