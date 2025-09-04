-- DDL Script for Medical System
-- PostgreSQL 15+

-- Creating PATIENTS table
CREATE TABLE patients (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    oib VARCHAR(11) UNIQUE NOT NULL,
    date_of_birth DATE NOT NULL,
    gender VARCHAR(1) NOT NULL CHECK (gender IN ('M', 'F')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    patient_number VARCHAR(20) -- Dodana kolona patient_number
);

-- Creating MEDICAL_DOCUMENTATION table (medical history)
CREATE TABLE medical_documentation (
    id SERIAL PRIMARY KEY,
    patient_id INTEGER NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
    disease_name VARCHAR(200) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NULL, -- NULL if disease is ongoing
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Creating EXAMINATIONS table
CREATE TABLE examinations (
    id SERIAL PRIMARY KEY,
    patient_id INTEGER NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
    examination_date DATE NOT NULL,
    examination_time TIME NOT NULL,
    examination_type VARCHAR(10) NOT NULL CHECK (examination_type IN (
        'GP', 'KRV', 'X-RAY', 'CT', 'MR', 'ULTRA', 'EKG', 'ECHO', 'EYE', 'DERM', 'DENTA', 'MAMMO', 'NEURO'
    )),
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Creating PRESCRIPTIONS table
CREATE TABLE prescriptions (
    id SERIAL PRIMARY KEY,
    patient_id INTEGER NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
    examination_id INTEGER REFERENCES examinations(id) ON DELETE SET NULL,
    medication_name VARCHAR(200) NOT NULL,
    dosage VARCHAR(100) NOT NULL,
    issue_date DATE NOT NULL,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Creating MEDICAL_IMAGES table
CREATE TABLE medical_images (
    id SERIAL PRIMARY KEY,
    examination_id INTEGER NOT NULL REFERENCES examinations(id) ON DELETE CASCADE,
    file_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    file_type VARCHAR(10) NOT NULL,
    file_size BIGINT NOT NULL,
    upload_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Creating indexes for better performance
CREATE INDEX idx_patients_oib ON patients(oib);
CREATE INDEX idx_patients_last_name ON patients(last_name);
CREATE INDEX idx_medical_documentation_patient_id ON medical_documentation(patient_id);
CREATE INDEX idx_examinations_patient_id ON examinations(patient_id);
CREATE INDEX idx_examinations_date ON examinations(examination_date);
CREATE INDEX idx_prescriptions_patient_id ON prescriptions(patient_id);
CREATE INDEX idx_medical_images_examination_id ON medical_images(examination_id);

-- Sample data for testing
INSERT INTO patients (first_name, last_name, oib, date_of_birth, gender) VALUES
('John', 'Doe', '12345678901', '1990-05-15', 'M'),
('Jane', 'Smith', '98765432109', '1985-10-22', 'F'),
('Mike', 'Johnson', '11223344556', '1975-03-08', 'M');

INSERT INTO medical_documentation (patient_id, disease_name, start_date, end_date) VALUES
(1, 'Hypertension', '2020-01-15', NULL),
(1, 'Common Cold', '2023-11-01', '2023-11-07'),
(2, 'Diabetes Type 2', '2019-06-20', NULL);

INSERT INTO examinations (patient_id, examination_date, examination_time, examination_type, description) VALUES
(1, '2024-01-15', '10:30:00', 'GP', 'Routine checkup'),
(1, '2024-01-20', '14:00:00', 'KRV', 'Blood test for cholesterol'),
(2, '2024-02-01', '09:00:00', 'X-RAY', 'Chest X-ray');

INSERT INTO prescriptions (patient_id, examination_id, medication_name, dosage, issue_date, notes) VALUES
(1, 1, 'Lisinopril', '10mg daily', '2024-01-15', 'Take with food'),
(2, 3, 'Metformin', '500mg twice daily', '2024-02-01', 'Monitor blood sugar levels');