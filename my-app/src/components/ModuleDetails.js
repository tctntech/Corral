import React, { useEffect, useState } from 'react';
import { getModuleById } from '../services/ModuleService';

const ModuleDetails = ({ moduleId }) => {
    const [module, setModule] = useState(null);

    useEffect(() => {
        if (moduleId) {
            fetchModuleDetails(moduleId);
        }
    }, [moduleId]);

    const fetchModuleDetails = async (id) => {
        try {
            const data = await getModuleById(id);
            console.log('Fetched Module Details:', data);
            if (data.success) {
                setModule(data.data);
            } else {
                console.error(data.message || 'Error fetching module details');
            }
        } catch (error) {
            console.error('Error fetching module details:', error);
        }
    };

    if (!module) {
        return <div>Select a module to view details</div>;
    }

    return (
        <div>
            <h3>Module Details</h3>
            <table>
                <tbody>
                    <tr>
                        <td>Module Name:</td>
                        <td>{module.moduleName}</td>
                    </tr>
                    <tr>
                        <td>Description:</td>
                        <td>{module.moduleDescription}</td>
                    </tr>
                    <tr>
                        <td>Form:</td>
                        <td>{module.moduleForm}</td>
                    </tr>
                    <tr>
                        <td>Parent:</td>
                        <td>{module.moduleParent}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
};

export default ModuleDetails;
