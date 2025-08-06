# 📋 TODO

---
This document is the official TODO list for the `VSLibrary.Threading` module,
tracking bugs, issues, and planned improvements.

## TODO List

| Status | Item Description                                                                            | Notes                                               |
| ------ | ------------------------------------------------------------------------------------------- | --------------------------------------------------- |
| ✅      | Fixed: Incorrect log context recorded when threads are split into separate files            | 🛠 by Minsu Jang                                    |
| ✅      | Removed direct reference from ThreadFactory to ThreadManager (resolved circular dependency) | 💡 Introduced IThreadRegistrar interface            |
| ✅      | Refactored: Moved CreateVirtualThread out of ThreadManagerProxy into VirtualThread          | Clarified responsibility and separation of concerns |

---

## 🗓️ Update History

| Date       | Author         | Description                |
|------------|---------------|----------------------------|
| 2025-06-17 | Jang Minsu    | Initial creation (Korean)  |
| 2025-06-26 | Jang Minsu    | Removed ThreadManager dependency from ThreadFactory, eliminating circular reference. |
| 2025-06-26 | Jang Minsu    | Verified and finalized class diagram after structural review.  |
| 2025-07-02 | ChatGPT (GPT-4)| Full English translation   |

---
📅 Document Date: 2025-06-17  
🖋️ Author: Minsu Jang

