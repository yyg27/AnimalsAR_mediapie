
# AnimalsAR with MediaPipe

This project is a real-time Mixed Reality interaction system built with **Unity 2022.3.62f2** and **MediaPipe** (Python). It allows you to use your hand to interact with various 3D animals (Chicken, Deer, Horse, Dog, etc.) in a virtual environment. The system uses **UDP** to stream hand landmark data from a Python bridge script directly into Unity.

-   Built as a final project with free assets for CEN451 - Introduction to Mixed Reality Applications Lecture

## Technologies Used

-   **Unity** (Version: 2022.3.62f2 LTS)
-   **C#**
-   **MediaPipe (Python)**
-   **Unity Asset Store** (for free assets)

## Installation

Due to Ubuntu 24.04's strict Python package rules, a **Virtual Environment (venv)** is required.

### 1. Clone the repository

```bash
git clone https://github.com/yyg27/AnimalsAR_mediapie.git
cd AnimalsAR_mediapie

```

### 2. Open the project in Unity Hub

Make sure Unity **2022.3.62f2 LTS** is installed.

### 3. Create and activate the virtual environment

Run these commands in the root folder (where `mp_to_unity.py` is located):

```bash
python3 -m venv venv
source venv/bin/activate

```

### 4. Install MediaPipe and OpenCV

```bash
pip install mediapipe==0.10.31 opencv-python

```

### 5. Run the Python script

```bash
python mp_to_unity.py

```

### 6. Start Unity

Press **Play** on the **MainScene** in Unity.

## Usage

Once both the Python script and Unity are running, your hand movements will be tracked via webcam and sent to Unity in real-time to interact with the 3D animals.

## Troubleshooting

-   If you get permission errors, make sure the virtual environment is activated (`source venv/bin/activate`)
-   If the camera doesn't open, check webcam permissions
-   Make sure Unity is running **after** starting the Python script

## Credits

Free assets used from Unity Asset Store.
