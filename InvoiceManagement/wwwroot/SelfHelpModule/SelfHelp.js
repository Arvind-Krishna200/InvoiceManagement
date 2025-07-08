class SelfHelpWidget {
    constructor(pageName = '', helpUrl = null) {
        this.pageName = pageName;
        this.helpUrl = `/SelfHelpModule/SelfHelp.json`;
        this.faqData = [];
        this.chatMessages = [];

        this.templates = {
            chatIntro: {
                role: "bot",
                type: "suggestions",
                text: "Hi! How can I help you? Try one of these:"
            },
            suggestions: [
                "How to reset my password?",
                "How to update my profile?",
                "Where to check application status?"
            ]
        };
    }

    async init() {
        await this.loadFaqData();
        this.renderFloatingIcon();
        this.renderPanel();
        document.querySelector(".self-help-icon").classList.add("glow");
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
        icon.innerHTML = `<span class="loader-box"><div class="loader"></div></span>`;
        icon.title = "Help & FAQ";
        icon.onclick = () => this.togglePanel(true);
        document.body.appendChild(icon);
    }

    togglePanel(show) {
        if (show) {
            this.panel.classList.add("open");
            document.querySelector(".self-help-icon").classList.remove("glow");
        } else {
            this.panel.classList.remove("open");
            document.querySelector(".self-help-icon").classList.add("glow");
        }
    }

    renderPanel() {
        const panel = document.createElement("div");
        panel.className = "self-help-panel";

        const faqHTML = this.faqData.map((item, i) => `
            <div class="faq-item">
                <div class="faq-question" data-index="${i}">
                    <span>${item.question}</span>
                    <span class="toggle-sign">+</span>
                </div>
                <div class="faq-answer">${item.answer}</div>
            </div>
        `).join("");

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
                ${faqHTML}
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

        this.attachEvents();
    }

    attachEvents() {
        this.panel.querySelector("#start-chat-btn").onclick = () => this.showChatView();
        this.panel.querySelector(".close-help").onclick = () => this.togglePanel(false);

        this.panel.querySelectorAll(".faq-question").forEach((q, index) => {
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

    showChatView() {
        this.toggleView(true);
        this.panel.querySelector(".back-to-faq").onclick = () => this.showFAQView();
        this.panel.querySelector("#chat-send").onclick = () => this.sendChatMessage();
        this.panel.querySelector("#chat-input").addEventListener("keydown", (e) => {
            if (e.key === "Enter") this.sendChatMessage();
        });

        if (this.chatMessages.length === 0) {
            this.chatMessages.push({
                role: "bot",
                type: "text",
                text: "I'm your AI assistant! Need help with something? You can start with one of these quick questions:",
                time: ''
            });
            this.chatMessages.push({
                role: "bot",
                type: "suggestions",
                text: '',
                suggestions: this.templates.suggestions,
                time: new Date()
            });
            this.renderMessages();
        }
    }

    showFAQView() {
        this.toggleView(false);
    }

    toggleView(toChat) {
        this.panel.querySelector(".self-help-body").style.display = toChat ? "none" : "block";
        this.panel.querySelector(".self-help-footer").style.display = toChat ? "none" : "flex";
        this.panel.querySelector(".self-help-search").style.display = toChat ? "none" : "block";
        this.panel.querySelector(".self-help-chat").style.display = toChat ? "flex" : "none";
        this.panel.querySelector(".self-help-title > .text").innerText = toChat ? "" : "FAQ";
        this.panel.querySelector(".back-to-faq").style.display = toChat ? "flex" : "none";
    }

    async sendChatMessage() {
        const input = this.panel.querySelector("#chat-input");
        const message = input.value.trim();
        if (!message) return;

        const now = new Date();
        this.chatMessages.push({ role: "user", text: message, time: now });
        input.value = "";
        this.renderMessages();

        this.appendLoader();

        try {
            const res = await fetch("/api/chat/reply", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ message })
            });
            const data = await res.json();

            this.removeLoader();
            this.chatMessages.push({ role: "bot", text: data.reply || "Sorry, I couldn’t understand that.", time: new Date() });
            this.renderMessages();
        } catch (err) {
            this.removeLoader();
            this.chatMessages.push({ role: "bot", text: "Oops! Something went wrong.", time: new Date() });
            this.renderMessages();
        }
    }

    appendLoader() {
        const chatBody = this.panel.querySelector("#chat-body");
        const loader = document.createElement("div");
        loader.className = "chat-msg bot chat-loader";
        loader.innerHTML = `
            <div class="chat-bubble">
                <div class="profile-pic">🤖</div>
                <div class="bubble-content">
                    <div class="chat-text loading-dots">
                        <span>.</span><span>.</span><span>.</span>
                    </div>
                </div>
            </div>
        `;
        chatBody.appendChild(loader);
        chatBody.scrollTop = chatBody.scrollHeight;
    }

    removeLoader() {
        const loader = this.panel.querySelector(".chat-loader");
        if (loader) loader.remove();
    }

    renderMessages() {
        const container = this.panel.querySelector("#chat-body");
        container.innerHTML = "";

        this.chatMessages.forEach(msg => {
            const msgDiv = document.createElement("div");
            msgDiv.className = `chat-msg ${msg.role}`;
            let time = '';
            if (msg.type === "suggestions") {
                msgDiv.innerHTML = this.getSuggestionHTML(msg.text, msg.suggestions,msg.time);
            } else {
                if (msg.time != '') {
                     time = new Date(msg.time).toLocaleTimeString([], {
                        hour: '2-digit',
                        minute: '2-digit'
                    });
                }

                msgDiv.innerHTML = `
                    <div class="chat-bubble">
                        <div class="profile-pic">${msg.role === 'bot' ? '🤖' : '🧑'}</div>
                        <div class="bubble-content">
                            <div class="chat-text">${msg.text}</div>
                            <div class="chat-time">${time}</div>
                        </div>
                    </div>
                `;
            }

            container.appendChild(msgDiv);
        });

        container.querySelectorAll(".suggestion-item").forEach(el => {
            el.addEventListener("click", (e) => {
                const q = e.currentTarget.dataset.question;
                this.panel.querySelector("#chat-input").value = q;
                this.sendChatMessage();
            });
        });

        container.scrollTo({
            top: container.scrollHeight,
            behavior: "smooth"
        });
    }

    getSuggestionHTML(title, suggestions = [], time) {
        const timeStr = new Date(time).toLocaleTimeString([], {
            hour: '2-digit',
            minute: '2-digit'
        });
        return `
            <div class="chat-bubble">
                <div class="profile-pic">🤖</div>
                <div class="bubble-content">
                    
                    <div class="chat-suggestions">
                        ${suggestions.map(s =>
            `<div class="suggestion-item" data-question="${s}">${s}</div>`
        ).join('')}
                    </div>
                     <div class="chat-time">${timeStr}</div>
                </div>
            </div>
        `;
    }
}

const widget = new SelfHelpWidget();
widget.init();
