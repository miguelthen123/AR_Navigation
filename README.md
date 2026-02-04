# AR Navigation

An **Augmented Reality (AR) navigation prototype** developed using **Unity and AR Foundation**.  
This project explores how AR-based wayfinding can enhance user experience in complex indoor environments such as hospitals, by overlaying contextual navigation information when reference images are detected.

---

## 📌 Project Overview

Navigating large hospital environments can be stressful and confusing for users.  
This project demonstrates a **proof-of-concept AR navigation system** that provides on-demand visual guidance using mobile AR, without requiring major physical renovations to the environment.

The system detects predefined reference images and overlays navigation-related AR content and UI elements accordingly.

---

## ✨ Features

- Image-based AR navigation using **AR Foundation**
- Dynamic spawning of AR objects based on tracked images
- Context-aware UI that updates according to detected image
- Persistent AR objects even when image tracking is temporarily lost
- Modular and extensible design for research and prototyping

---

## 🛠️ Built With

- **Unity** (recommended: latest LTS)
- **AR Foundation**
- **ARCore XR Plugin / ARKit XR Plugin**
- **TextMeshPro**
- **C#**

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:

- Unity Hub
- Unity Editor (LTS version recommended)
- Mobile device supporting:
  - ARCore (Android) or
  - ARKit (iOS)

---

### Clone the Repository

```bash
git clone https://github.com/miguelthen123/AR_Navigation.git
cd AR_Navigation
