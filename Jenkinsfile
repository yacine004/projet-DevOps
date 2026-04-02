pipeline {
    agent any

    stages {

        stage('Clone') {
            steps {
                git 'https://github.com/yacine004/projet-DevOps.git'
            }
        }

        stage('Build .NET') {
            steps {
                bat 'dotnet build'
            }
        }

        stage('Build Docker') {
            steps {
                bat 'docker build -t brasilburger .'
            }
        }

        stage('Run Container') {
            steps {
                bat 'docker run -d -p 8081:8080 brasilburger'
            }
        }
    }
}