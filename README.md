# Sliding Puzzle Master 🧩

A modern Windows Desktop application built with **WPF (.NET)** that allows users to play and solve sliding tile puzzles. The project features an intelligent "Auto-Solve" engine that compares different Artificial Intelligence search algorithms.



## ✨ Features
*   **Customizable Board:** Support for puzzle dimensions from 2x2 up to 4x4.
*   **Interactive UI:** Modern, responsive design with smooth tile interactions and visual hints.
*   **AI Solver:** Multiple search algorithms to find the optimal path to the solution.
*   **Performance Dashboard:** Real-time analysis of memory usage, nodes expanded, and execution time.

## 🤖 Supported Algorithms
The application implements three core search strategies to explore the state space:

| Algorithm | Description | Optimization |
| :--- | :--- | :--- |
| **A* Search** | Uses both path cost ($g$) and Manhattan distance ($h$) | Most Efficient |
| **Greedy Search** | Prioritizes states closest to the goal ($h$ only) | Fast but not always optimal |
| **Uniform Cost** | Expands nodes based on path cost ($g$) | Guaranteed optimal, but slow |

## 🛠️ Technical Details
*   **Heuristic:** The solver utilizes the **Manhattan Distance** formula to estimate the cost to the goal state.
*   **State Management:** High-performance grid serialization using `long` bit-shifting for rapid "visited state" lookups.
*   **Asynchronous Processing:** Solvers run on background threads using `Task.Run` and `CancellationToken` to keep the UI responsive during complex calculations.

## 🚀 Getting Started
1. Clone the repository.
2. Open the solution in **Visual Studio 2022**.
3. Restore NuGet packages and Build.
4. Run the `SlidingPuzzle.WPF` project.

---

### How to Play
1. Enter your desired **Dimensions** (e.g., 3x3).
2. Click **Shuffle** to randomize the board.
3. Click tiles adjacent to the empty slot to move them, or select an algorithm and click **Solve Auto** to watch the AI work!

---
*Developed as a demonstration of Search Algorithms and WPF UI Design.*