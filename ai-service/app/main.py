from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.routers import ai_router

app = FastAPI(
    title="Expense Tracker AI Service",
    description="AI-powered insights for expense tracking",
    version="1.0.0"
)

# CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000", "http://localhost:5000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Routers
app.include_router(ai_router.router, prefix="/ai", tags=["AI"])

@app.get("/")
def root():
    return {"message": "Expense Tracker AI Service", "status": "running"}

@app.get("/health")
def health_check():
    return {"status": "healthy"}
