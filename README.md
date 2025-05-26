# 🏀 AR Basketball

**AR Basketball** is a mobile Augmented Reality game that lets you place a virtual basketball hoop into your real environment and shoot hoops with intuitive swipe controls. Built with Unity and AR Foundation, it brings classic arcade basketball action into your own space.

---

## 🎯 Game Overview

Using your phone’s camera, a hoop is placed in the real world via AR. Players swipe up on the screen to shoot a basketball toward the hoop. The trajectory and velocity of the ball are dynamically calculated based on the **direction**, **length**, and **speed** of the swipe.

---

## 🕹️ Core Gameplay Features

- **AR Hoop Placement**  
  - Detects horizontal planes and anchors the basketball hoop in front of the player.
  
- **Swipe-to-Shoot Controls**  
  - Swipe gesture mapped to:
    - **Swipe direction** → ball launch angle  
    - **Swipe length** → initial force  
    - **Swipe speed** → power and velocity

- **Realistic Ball Physics**  
  - Gravity, arc trajectory, and rim interaction provide a satisfying feel.
  
- **Simple Retry Loop**  
  - After a shot, a new ball spawns automatically for continuous play.
