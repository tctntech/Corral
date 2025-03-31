import React, { useState } from 'react';
import ModuleList from '../components/ModuleList';
import ModuleDetails from '../components/ModuleDetails';

const ModuleManagement = ({ managerID }) => {
    const [selectedModule, setSelectedModule] = useState(null);

    const handleSelectModule = (moduleId) => {
        setSelectedModule(moduleId);
    };

    return (
        <div>
            <h2>Module Management</h2>
            <ModuleList managerID={managerID} onSelectModule={handleSelectModule} />
            {selectedModule && <ModuleDetails moduleId={selectedModule} />}
        </div>
    );
};

export default ModuleManagement;
