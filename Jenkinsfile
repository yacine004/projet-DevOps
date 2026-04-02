pipeline {
    agent any

    options {
        skipDefaultCheckout()
        disableConcurrentBuilds()
    }

    stages {

        stage('Checkout') {
            steps {
                checkout([
                    $class: 'GitSCM',
                    branches: [[name: '*/main']],
                    userRemoteConfigs: [[url: 'https://github.com/yacine004/projet-DevOps.git']]
                ])
            }
        }

        stage('Build .NET') {
            steps {
                bat 'cd csharp_web && dotnet build --configuration Release'
            }
        }

        stage('Build Docker') {
            steps {
                bat 'docker build -t brasilburger .'
            }
        }

        stage('Run Container') {
            steps {
                bat 'docker stop brasilburger || exit 0'
                bat 'docker rm brasilburger || exit 0'
                bat 'docker run -d --name brasilburger -p 8081:8080 brasilburger'
            }
        }
    }
}