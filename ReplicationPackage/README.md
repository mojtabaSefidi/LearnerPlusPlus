# Replication Package

The overall steps are

1. Install Relational Git
2. Get the Database
3. Run the Simulations (seeded random or random). Please take a look at the TSE paper for more details on why we used seeded random.
4. Dump the Simulation Data to CSV
5. Calculate the Expertise, Workload, and FaR measures

## Install Relational Git

1) [Install](https://github.com/fahimeh1368/SofiaWL/blob/gh-pages/install.md) the tool and its dependencies.

## Get the Database

1) Restore the data backup to an MS SQL Server. Each studied project has a separate database. You can select individual files from the [db backup](https://www.dropbox.com/home/SofiaWL-Databases). Note that some files are over 15GB.
2) Copy the [configuration files](config).
3) Open and modify each configuration file to set the connection string. You need to provide the server address along with the credentials. The following snippet shows a sample of how the connection string should be set.

```json
 {
	"ConnectionStrings": {
	  "RelationalGit": "Server=ip_db_server;User Id=user_name;Password=pass_word;Database=coreclr"
	},
	"Mining":{
 		
  	}
 }
```

4) Open [simulations.ps1](simulations.ps1) using an editor and make sure the config variables defined at the top of the file refer to the correct location of the downloaded config files. 

```powershell
# Each of the following variables contains the absolute path of the corresponding configuation file.
$corefx_conf = "absolute/path/to/corefx_conf.json"
$coreclr_conf = "absolute/path/to/coreclr_conf.json"
$roslyn_conf = "absolute/path/to/roslyn_conf.json"
$rust_conf = "absolute/path/to/rust_conf.json"
$kubernetes_conf = "absolute/path/to/kubernetes_conf.json"
```

## Run the Simulations

1) Run the [simulations.ps1](simulations.ps1) script. Open PowerShell and run the following command in the directory of the file

``` powershell
./simulations.ps1
```

This scripts runs all the defined reviewer recommendation algorithms accross all projects. Each run is called a simulation because for each pull request one of the actual reviewers is randomly selected to be replaced by the top recommended reviewer.

**Note**: Make sure you have set the PowerShell [execution policy](https://superuser.com/questions/106360/how-to-enable-execution-of-powershell-scripts) to **Unrestricted** or **RemoteAssigned**.

## Research Questions

The following sections show which simulations are used for each research question.

### Simulation RQ1, Replication: Which existing reviewer recommender spreads knowledge most efficiently among developers?

**Note**: In order to select between ```Random``` and ```SeededRandom```, use the ```--simulation-type``` command. If you want to run the seeded version, set the value of ```--simulation-type``` to ```Random``` for **cHRev** and all the other algorithms to ```SeededRandom```. If you wish to run the random version, set the value of ```--simulation-type``` to ```Random``` for all the algorithms.

```PowerShell
# Reality
dotnet-rgit --cmd simulate-recommender --recommendation-strategy Reality --conf-path <path_to_config_file>
# cHRev Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy cHRev --simulation-type "Random" --conf-path <path_to_config_file>
# AuthorshipRec Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy AuthorshipRec --simulation-type "SeededRandom" --conf-path <path_to_config_file>
# RevOwnRec Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy RevOwnRec --simulation-type "SeededRandom" --conf-path <path_to_config_file>
# LearnRec Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy LearnRec --simulation-type "SeededRandom" --conf-path <path_to_config_file>
# RetentionRec Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy RetentionRec --simulation-type "SeededRandom" --conf-path <path_to_config_file>
# TurnoverRec Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_config_file>
# Sofia Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy Sofia --simulation-type "SeededRandom" --conf-path <path_to_config_file>
#WhoDo recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy WhoDo --simulation-type "SeededRandom" --conf-path <path_to_config_file>
# SofiaWL Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy SofiaWL --simulation-type "SeededRandom" --conf-path <path_to_config_file>
```
---

**Note:** To run the simulations for each of the following research questions, you need to change the config file of all five projects. We suggest creating an exclusive config file for each research question to avoid confusion.

### Simulation RQ2, Recommenders++: How does adding an additional reviewer for risky pull requests affect the outcome measures?

To run the Recommenders++ strategy, you should apply the following changes to the config file of each project.

```
"PullRequestReviewerSelectionStrategy" : "0:nothing-nothing,-:add-1",
```

In the next step, run simulation for each recommender. Since the Recommenders++ strategy suggests an additional learner for risky pull requests and does not replace any reviewer, there is no need to use the ```--simulation-type``` command.

```PowerShell
# AuthorshipRec++ Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy AuthorshipRec --conf-path <path_to_config_file>
# RevOwnRec++ Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy RevOwnRec --conf-path <path_to_config_file>
# cHRev++ Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy cHRev --conf-path <path_to_config_file>
# LearnRec++ Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy LearnRec --conf-path <path_to_config_file>
# RetentionRec++ Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy RetentionRec --conf-path <path_to_config_file>
# TurnoverRec++ Recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --conf-path <path_to_config_file>
# WhoDo++ recommender
dotnet-rgit --cmd simulate-recommender --recommendation-strategy WhoDo --conf-path <path_to_config_file>
```
---


### Simulation RQ3, FaR-Aware: How effective is the combined use of TurnoverRec++ for abandoned files and random replacement for hoarded files in spreading knowledge?

To run the FaR-Aware approach, you should apply the following changes to the config files.

```
"PullRequestReviewerSelectionStrategy" : "0:nothing-nothing,-:addAndReplace-1",
```

Then, you should run the FaR-Aware recommender for each project. The ```--simulation-type``` command forces the recommender to replace the same reviewer in all the simulations.

```PowerShell
# FaR-Aware recommender for CoreFX
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_CoreFX_config_file>
# FaR-Aware recommender for CoreCLR
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_CoreCLR_config_file>
# FaR-Aware recommender for Roslyn
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_Roslyn_config_file>
# FaR-Aware recommender for Rust
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_Rust_config_file>
# FaR-Aware recommender for Kubernetes
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_Kubernetes_config_file>
```
---


### Simulation RQ4, Hoarded-X: How can we control the trade-off between ΔFaR and AddedPR when we recommend an additional learner for risky pull requests?

To run the Hoarded-X strategy, you should apply the following changes to the config file of each project.

```
"PullRequestReviewerSelectionStrategy" : "0:nothing-nothing,-:addHoarded_X-1",
```

The X parameter should be adjusted based on the recommender. In our paper, we run simulations for X = {2,3,4}. For example, if you want to run the **Hoarded-2** recommender, you should change the config files as follows:

```
"PullRequestReviewerSelectionStrategy" : "0:nothing-nothing,-:addHoarded_2-1",
```

After adjusting the config files for all projects, you should run the Hoarded-X approach for each project. 

```PowerShell
# Hoarded-X recommender for CoreFX
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_CoreFX_config_file>
# Hoarded-X recommender for CoreCLR
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_CoreCLR_config_file>
#Hoarded-X recommender for Roslyn
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_Roslyn_config_file>
# Hoarded-X recommender for Rust
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_Rust_config_file>
# Hoarded-X recommender for Kubernetes
dotnet-rgit --cmd simulate-recommender --recommendation-strategy TurnoverRec --simulation-type "SeededRandom" --conf-path <path_to_Kubernetes_config_file>
```
---

## Dump the Simulation Data to CSV

Log into the database and run the following command to find the IDs of your simulation.

```SQL
-- Get the Id of the simulation 
select Id, KnowledgeShareStrategyType, StartDateTime from LossSimulations
```

Substitute the ```<rec_sim_id>``` variable with the Id of your desired recommender,  and compare the recommender performance with the actual values, ```<reality_id>```. You should also substitute ```<path_to_result>``` variable with the path where you want to save the results. 

```PowerShell
dotnet-rgit --cmd analyze-simulations --analyze-result-path <path_to_result> --recommender-simulation <rec_sim_id> --reality-simulation <reality_id>  --conf-path <path_to_config_file>
```

### Results for Expertise, FaR, and Gini-Workload outcomes:

After running the analyzer, the tool creates four CSV files: **expertise.csv**, **far.csv**, **workload.csv**, and **auc.csv**. The first column shows the project's periods (quarters) in the first three files. Each column corresponds to one of the simulations. Each cell shows the percentage change between the actual and simulated outcomes in that period. The last row of a column shows the median of its values. The file **auc.csv** from the prior step indicates the number of developer reviews in each quarter. To plot the Lorenz-Curve for each recommender, you should run [WorkloadAUC.r](WorkloadMeasures/WorkloadAUC.R).
