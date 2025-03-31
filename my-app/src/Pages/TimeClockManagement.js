import React, { useState } from "react";
import TimeClockList from "../components/TimeClockList";
import TimeClockDetails from "../components/TimeClockDetails";

const TimeClockManagement = () => {
    const [selectedUserId, setSelectedUserId] = useState(null);

    return (
        <div style={{ display: 'flex', justifyContent: 'space-between', gap: '20px' }}>
            <TimeClockList onSelectTimeClock={setSelectedUserId} />
            {selectedUserId && <TimeClockDetails userId={selectedUserId} />}
        </div>
    );
};

export default TimeClockManagement;
