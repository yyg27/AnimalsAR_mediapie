import cv2
import socket
import mediapipe as mp

mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

UDP_IP = "127.0.0.1"
UDP_PORT = 5005
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

hands = mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=2,
    min_detection_confidence=0.7,
    min_tracking_confidence=0.5
)

cap = cv2.VideoCapture(0)
print("Bridge initiated! Show your hand to the camera...")

while cap.isOpened():
    success, image = cap.read()
    if not success: continue

    image = cv2.flip(image, 1)
    results = hands.process(cv2.cvtColor(image, cv2.COLOR_BGR2RGB))

    l_hand, r_hand = "0,0,0,0", "0,0,0,0"

    if results.multi_hand_landmarks:
        for i, res in enumerate(results.multi_hand_landmarks):
            tip = res.landmark[8]
            data = f"{tip.x:.4f},{tip.y:.4f},{tip.z:.4f},1"
            
            label = results.multi_handedness[i].classification[0].label
            if label == "Left": l_hand = data
            else: r_hand = data
            
            mp_drawing.draw_landmarks(image, res, mp_hands.HAND_CONNECTIONS)

    sock.sendto(f"{l_hand}|{r_hand}".encode(), (UDP_IP, UDP_PORT))

    cv2.imshow('Garrix Tracking', image)
    if cv2.waitKey(1) & 0xFF == 27: break

cap.release()
cv2.destroyAllWindows()
