import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import UserManagement from './Pages/UserManagement';
import ModuleManagement from './Pages/ModuleManagement';
import PermissionManagement from './Pages/PermissionManagement';

import TimeClockManagement from './Pages/TimeClockManagement';
import AbsoluteProducts from "./components/AbsoluteProducts";
function App() {
    return (
        <Router>
            <Routes>
                <Route path="/users" element={<UserManagement />} />
                <Route path="/modules/:managerID" element={<ModuleManagement />} />
                <Route path="/permissions/:managerID" element={<PermissionManagement />} />
                <Route path="/timeclock" element={<TimeClockManagement />} />
                <Route path="/absolute-products" element={<AbsoluteProducts />} />
              
            </Routes>
        </Router>
    );
}

