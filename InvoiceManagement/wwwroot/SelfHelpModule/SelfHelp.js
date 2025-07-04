class SelfHelpWidget {
    constructor(pageName = '', helpUrl = null) {
        this.pageName = pageName;
        this.helpUrl = `/SelfHelpModule/SelfHelp.json`;
        this.faqData = [];
        this.chatMessages = [];
    }

    async init() {
        await this.loadFaqData();
        this.renderFloatingIcon();
        this.renderPanel();
    }

    async loadFaqData() {
        try {
            const res = await fetch(this.helpUrl);
            this.faqData = await res.json();
        } catch (err) {
            console.error("Failed to load FAQ data:", err);
        }
    }

    renderFloatingIcon() {
        const icon = document.createElement("div");
        icon.className = "self-help-icon";
        icon.innerHTML = `<span class="material-symbols-outlined" style="color:white;">forum</span>`;
        icon.title = "Help & FAQ";
        icon.onclick = () => this.togglePanel(true);
        document.body.appendChild(icon);
    }

    renderPanel() {
        const panel = document.createElement("div");
        panel.className = "self-help-panel";
        panel.innerHTML = `
           <div class="self-help-header">
                <span class="self-help-title">
                    <span class="back-to-faq material-symbols-outlined" style="display:none;"> arrow_left_alt </span>
                    <span class="text">FAQ</span>
                </span>
                <span class="close-help"><span class="material-symbols-outlined"> close_small </span></span>
            </div>
            <div class="self-help-body">
               <div class="self-help-search">
                  <span class="search-icon material-symbols-outlined">search</span>
                  <input type="text" placeholder="Search FAQs..." id="self-help-search-input">
                </div>
                ${this.faqData.map((item, i) => `
                    <div class="faq-item">
                        <div class="faq-question" data-index="${i}">
                            <span>${item.question}</span>
                            <span class="toggle-sign">+</span>
                        </div>
                        <div class="faq-answer">${item.answer}</div>
                    </div>
                `).join("")}
            </div>
           <div class="self-help-chat">
                <div class="chat-body" id="chat-body"></div>
                <div class="chat-input-area">
                    <input type="text" id="chat-input" placeholder="Type your message..." />
                    <button id="chat-send"><span class="material-symbols-outlined">send</span></button>
                </div>
            </div>
            <div class="self-help-footer">
                <div id="start-chat-btn">Chat with us</div>
            </div>
        `;
        document.body.appendChild(panel);
        this.panel = panel;
        panel.querySelector("#start-chat-btn").onclick = () => this.showChatView();

        // Close
        panel.querySelector(".close-help").onclick = () => this.togglePanel(false);

        // Toggle Q&A
        panel.querySelectorAll(".faq-question").forEach((q, index) => {
            const answer = q.nextElementSibling;
            const sign = q.querySelector(".toggle-sign");

           
            if (index <= 2) {
                answer.style.display = "block";
                if (sign) sign.textContent = "-";
            }

          
            q.onclick = () => {
                const open = answer.style.display === "block";
                answer.style.display = open ? "none" : "block";
                if (sign) sign.textContent = open ? "+" : "-";
            };
        });
    }

    togglePanel(show) {
        if (this.panel) {
            this.panel.style.display = show ? "flex" : "none";
        }
    }
    togglePanel(show) {
        if (show) {
            this.panel.classList.add("open");
        } else {
            this.panel.classList.remove("open");
        }
    }

    showChatView() {
        this.panel.querySelector(".self-help-body").style.display = "none";
        this.panel.querySelector(".self-help-footer").style.display = "none";
        this.panel.querySelector(".self-help-search").style.display = "none";
        this.panel.querySelector(".self-help-chat").style.display = "flex";

        // Toggle header
        this.panel.querySelector(".self-help-title > .text").innerText = "Live Chat";
        this.panel.querySelector(".back-to-faq").style.display = "flex";

        // Setup events once
        this.panel.querySelector(".back-to-faq").onclick = () => this.showFAQView();
        this.panel.querySelector("#chat-send").onclick = () => this.sendChatMessage();
        this.panel.querySelector("#chat-input").addEventListener("keydown", (e) => {
            if (e.key === "Enter") this.sendChatMessage();
        });
        const chatBody = this.panel.querySelector("#chat-body");
        if (this.chatMessages.length === 0) {
            this.chatMessages.push({
                role: "bot",
                text: "Hi there! How can I assist you today?",
                time: new Date()
            });
            this.renderMessages();
        }
    }


    showFAQView() {
        this.panel.querySelector(".self-help-body").style.display = "block";
        this.panel.querySelector(".self-help-footer").style.display = "flex";
        this.panel.querySelector(".self-help-search").style.display = "block";
        this.panel.querySelector(".self-help-chat").style.display = "none";

        // Reset header
        this.panel.querySelector(".self-help-title > .text").innerText = "FAQ";
        this.panel.querySelector(".back-to-faq").style.display = "none";
    }


    sendChatMessage() {
        const input = this.panel.querySelector("#chat-input");
        const message = input.value.trim();
        if (!message) return;

        const now = new Date();

        // Push user message
        this.chatMessages.push({
            role: "user",
            text: message,
            time: now
        });

        input.value = "";
        this.renderMessages();

        // bot reply
        setTimeout(() => {
            const reply = this.generateBotReply(message);
            this.chatMessages.push({
                role: "bot",
                text: reply,
                time: new Date()
            });
            this.renderMessages();
        }, 600);
    }

    renderMessages() {
        const container = this.panel.querySelector("#chat-body");
        container.innerHTML = "";

        this.chatMessages.forEach(msg => {
            const msgDiv = document.createElement("div");
            msgDiv.className = `chat-msg ${msg.role}`;

            // Format timestamp
            const time = new Date(msg.time).toLocaleTimeString([], {
                hour: '2-digit',
                minute: '2-digit'
            });


            msgDiv.innerHTML = `
                <div class="chat-bubble">
                    <div class="profile-pic">
                        ${msg.role === 'bot' ? '🤖' : '🧑'} 
                    </div>
                    <div class="bubble-content">
                        <div class="chat-text">${msg.text}</div>
                        <div class="chat-time">${time}</div>
                    </div>
                </div>
            `;

            container.appendChild(msgDiv);
        });

        container.scrollTo({
            top: container.scrollHeight,
            behavior: "smooth"
        });
    }


    generateBotReply(input) {
        // Simple static rules 
        const lower = input.toLowerCase();
        if (lower.includes("reset")) return "To reset your password, click 'Forgot Password'.";
        if (lower.includes("account")) return "You can manage account settings from the Profile page.";
        return "Thanks for your message! A team member will reply shortly.";
    }

}

const widget = new SelfHelpWidget();
widget.init();

