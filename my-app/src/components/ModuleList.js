import React, { useEffect, useState } from 'react';
import { getModulesByManagerId } from '../services/ModuleService';

const ModuleList = ({ managerID, onSelectModule }) => {
    const [modules, setModules] = useState([]);

    useEffect(() => {
        if (managerID) {
            fetchModules(managerID);
        }
    }, [managerID]);

    const fetchModules = async (id) => {
        try {
            const data = await getModulesByManagerId(id);
            console.log('Fetched modules:', data);
            if (data.success) {
                setModules(data.data); // Assuming 'data' is inside a 'data' object
            } else {
                console.error(data.message || 'Error fetching modules');
            }
        } catch (error) {
            console.error('Error fetching modules:', error);
        }
    };

    return (
        <div>
            <h3>Module List</h3>
            <table>
                <thead>
                    <tr>
                        <th>Module Name</th>
                        <th>Description</th>
                        <th>Form</th>
                        <th>Parent</th>
                    </tr>
                </thead>
                <tbody>
                    {modules.length > 0 ? (
                        modules.map((module) => (
                            <tr key={module.moduleID} onClick={() => onSelectModule(module.moduleID)}>
                                <td>{module.moduleName}</td>
                                <td>{module.moduleDescription}</td>
                                <td>{module.moduleForm}</td>
                                <td>{module.moduleParent}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="4">No modules available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ModuleList;
